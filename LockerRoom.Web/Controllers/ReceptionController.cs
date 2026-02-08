using LockerRoom.Core.Entities;
using LockerRoom.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LockerRoom.Web.Controllers
{
    public class ReceptionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPOSService _posService;

        public ReceptionController(IUnitOfWork unitOfWork, IPOSService posService)
        {
            _unitOfWork = unitOfWork;
            _posService = posService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Rooms(string gender)
        {
            ViewBag.Gender = gender ?? "Male";

            var lockers = await _unitOfWork.Lockers.GetAllAsync();
            var filteredLockers = lockers
                .Where(l => l.Gender == (gender ?? "Male"))
                .ToList();

            return View(filteredLockers);
        }

        [HttpGet]
        public async Task<IActionResult> GetLockersStatus(string gender)
        {
            var lockers = await _unitOfWork.Lockers.GetAllAsync();
            var filtered = lockers
                .Where(l => l.Gender == (gender ?? "Male"))
                .Select(l => new { l.LockerID, l.LockerCode, l.IsOccupied })
                .ToList();

            return Json(filtered);
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestCurrency()
        {
            var rates = await _unitOfWork.CurrencyRates.GetAllAsync();
            var latest = rates.OrderByDescending(r => r.RateDate).FirstOrDefault();
            if (latest == null) return NotFound();
            return Json(new { id = latest.Id, name = latest.CurrencyName, amount = latest.Amount });
        }

        [HttpPost]
        public async Task<IActionResult> StartBooking([FromBody] StartBookingRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.BraceletCode))
                return BadRequest("invalid");

            var locker = await _unitOfWork.Lockers.GetByIdAsync(req.LockerId);
            if (locker == null) return NotFound("locker");
            if (locker.IsOccupied) return Conflict("occupied");

            var rates = await _unitOfWork.CurrencyRates.GetAllAsync();
            var latest = rates.OrderByDescending(r => r.RateDate).FirstOrDefault();

            return Json(new
            {
                lockerId = locker.LockerID,
                lockerCode = locker.LockerCode,
                groupId = locker.GroupID,
                currency = latest == null ? null : new { id = latest.Id, name = latest.CurrencyName, amount = latest.Amount }
            });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking([FromBody] ConfirmBookingRequest req)
        {
            if (req == null) return BadRequest();

            var locker = await _unitOfWork.Lockers.GetByIdAsync(req.LockerId);
            if (locker == null) return NotFound("locker");
            if (locker.IsOccupied) return Conflict("occupied");

            var username = HttpContext.Session.GetString("Username") ?? "System";

            // ✅ حاول تتصل بالـ POS
            int? checkNo = null;
            string posError = null;

            try
            {
                checkNo = await _posService.CreateCheckWithOrderAsync(
                    locker.LockerCode,
                    locker.Gender,
                    req.Amount,
                    username,
                    req.BraceletCode
                );

                if (checkNo == null || checkNo == 0)
                {
                    posError = "POS returned null or zero";
                }
            }
            catch (Exception ex)
            {
                posError = ex.Message;
            }

            // ✅ احفظ الحجز (حتى لو POS فشل)
            var booking = new Booking
            {
                LockerID = locker.LockerID,
                BraceletCode = req.BraceletCode,
                CurrencyID = req.CurrencyId,
                Amount = req.Amount,
                PaymentType = req.PaymentType,
                BookingDate = DateTime.UtcNow,
                IsActive = true,
                CreatedBy = username,
                POSCheckNo = checkNo ?? 0
            };

            await _unitOfWork.Bookings.AddAsync(booking);
            locker.IsOccupied = true;
            _unitOfWork.Lockers.Update(locker);
            await _unitOfWork.CompleteAsync();

            // ✅ Log
            var log = new LockerLog
            {
                ActionType = "BookingConfirmed",
                Username = username,
                LockerID = locker.LockerID,
                BookingID = booking.BookingID,
                BraceletCode = booking.BraceletCode,
                CurrencyID = booking.CurrencyID,
                Amount = booking.Amount,
                PaymentType = booking.PaymentType,
                Details = checkNo != null && checkNo > 0
                    ? $"Locker {locker.LockerCode} booked. POS Check: {checkNo}"
                    : $"Locker {locker.LockerCode} booked. POS Error: {posError ?? "Unknown"}",
                CreatedBy = username,
                ActionDate = DateTime.Now
            };
            await _unitOfWork.LockerLogs.AddAsync(log);
            await _unitOfWork.CompleteAsync();

            return Json(new
            {
                success = true,
                bookingId = booking.BookingID,
                lockerId = locker.LockerID,
                isOccupied = true,
                posCheckNo = checkNo,
                posStatus = checkNo > 0 ? "synced" : "failed",
                posError = posError
            });
        }

        [HttpPost]
        public async Task<IActionResult> ManualUnlock([FromBody] int lockerId)
        {
            var username = HttpContext.Session.GetString("Username") ?? "System";
            var now = DateTime.Now;

            var locker = await _unitOfWork.Lockers.GetByIdAsync(lockerId);
            if (locker == null) return NotFound("Locker not found");

            var booking = (await _unitOfWork.Bookings.FindAsync(b => b.LockerID == lockerId && b.IsActive))
                .OrderByDescending(b => b.BookingDate)
                .FirstOrDefault();

            if (booking != null)
            {
                booking.IsActive = false;
                booking.EndDate = now;
                booking.CreatedBy = username;
                _unitOfWork.Bookings.Update(booking);
            }

            locker.IsOccupied = false;
            _unitOfWork.Lockers.Update(locker);
            await _unitOfWork.CompleteAsync();

            var log = new LockerLog
            {
                ActionType = "ManualUnlock",
                Username = username,
                LockerID = locker.LockerID,
                BookingID = booking?.BookingID,
                BraceletCode = booking?.BraceletCode,
                PreviousIsActive = booking?.IsActive,
                PreviousEndDate = booking?.EndDate,
                Details = $"Locker {locker.LockerCode} manually unlocked by {username}.",
                CreatedBy = username,
                ActionDate = now
            };
            await _unitOfWork.LockerLogs.AddAsync(log);
            await _unitOfWork.CompleteAsync();

            return Json(new { success = true, message = $"Locker {locker.LockerCode} unlocked by {username} at {now}" });
        }

        [HttpGet]
        public async Task<IActionResult> GetOccupiedLockers()
        {
            var lockers = await _unitOfWork.Lockers.FindAsync(l => l.IsOccupied);
            var result = lockers.Select(l => new { l.LockerID, l.LockerCode }).ToList();
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> FocusScan([FromBody] FocusScanRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.BraceletCode)) return BadRequest();

            var bookings = await _unitOfWork.Bookings.FindAsync(b => b.BraceletCode == req.BraceletCode && b.IsActive);
            var booking = bookings.OrderByDescending(b => b.BookingDate).FirstOrDefault();

            if (booking == null) return NotFound();

            var expiry = booking.BookingDate.AddHours(24);

            return Json(new
            {
                booking.BookingID,
                booking.LockerID,
                booking.BraceletCode,
                booking.Amount,
                booking.PaymentType,
                booking.BookingDate,
                expiresAt = expiry
            });
        }
    }

    // DTOs
    public class StartBookingRequest
    {
        public int LockerId { get; set; }
        public string BraceletCode { get; set; } = string.Empty;
    }

    public class ConfirmBookingRequest
    {
        public int LockerId { get; set; }
        public string BraceletCode { get; set; } = string.Empty;
        public int CurrencyId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; } = "Cash";
    }

    public class FocusScanRequest
    {
        public string BraceletCode { get; set; } = string.Empty;
    }
}