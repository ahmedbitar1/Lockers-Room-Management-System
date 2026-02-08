using Microsoft.AspNetCore.Mvc;
using LockerRoom.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LockerRoom.Web.Controllers
{
    public class ScreenController : Controller
    {
        private readonly MainDbContext _context;

        public ScreenController(MainDbContext context)
        {
            _context = context;
        }

        // صفحة بسيطة فيها زرار Scan
        public IActionResult Index()
        {
            return View();
        }

        // API لجلب تفاصيل البريسلت
        [HttpGet]
        public async Task<IActionResult> GetBooking(string bracelet)
        {
            if (string.IsNullOrEmpty(bracelet))
                return Json(new { success = false });

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BraceletCode == bracelet && b.IsActive == true);

            if (booking == null)
                return Json(new { success = false });

            var locker = await _context.Lockers
                .FirstOrDefaultAsync(l => l.LockerID == booking.LockerID);

            if (locker == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                groupId = locker.GroupID,
                lockerCode = locker.LockerCode,
                gender = locker.Gender
            });
        }
    }
}
