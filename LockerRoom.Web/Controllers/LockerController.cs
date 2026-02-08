using LockerRoom.Core.Entities;
using LockerRoom.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LockerRoom.Web.Controllers
{
    public class LockerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LockerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var lockers = await _unitOfWork.Lockers.GetAllAsync();
            return View(lockers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var locker = await _unitOfWork.Lockers.GetByIdAsync(id);
            if (locker == null)
                return NotFound();

            return View(locker);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Locker locker)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Lockers.AddAsync(locker);
                await _unitOfWork.CompleteAsync();

                var username = HttpContext.Session.GetString("Username") ?? "System";
                var log = new LockerLog
                {
                    ActionType = "CreateLocker",
                    Username = username,
                    LockerID = locker.LockerID,
                    Details = $"Locker {locker.LockerCode} created.",
                    CreatedBy = username,
                    ActionDate = DateTime.Now
                };
                await _unitOfWork.LockerLogs.AddAsync(log);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(locker);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var locker = await _unitOfWork.Lockers.GetByIdAsync(id);
            if (locker == null)
                return NotFound();

            return View(locker);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Locker locker)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Lockers.Update(locker);
                await _unitOfWork.CompleteAsync();

                var username = HttpContext.Session.GetString("Username") ?? "System";
                var log = new LockerLog
                {
                    ActionType = "EditLocker",
                    Username = username,
                    LockerID = locker.LockerID,
                    Details = $"Locker {locker.LockerCode} updated.",
                    CreatedBy = username,
                    ActionDate = DateTime.Now
                };
                await _unitOfWork.LockerLogs.AddAsync(log);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(locker);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var locker = await _unitOfWork.Lockers.GetByIdAsync(id);
            if (locker == null)
                return NotFound();

            return View(locker);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locker = await _unitOfWork.Lockers.GetByIdAsync(id);
            if (locker == null)
                return NotFound();

            _unitOfWork.Lockers.Remove(locker);
            await _unitOfWork.CompleteAsync();

            var username = HttpContext.Session.GetString("Username") ?? "System";
            var log = new LockerLog
            {
                ActionType = "DeleteLocker",
                Username = username,
                LockerID = locker.LockerID,
                Details = $"Locker {locker.LockerCode} deleted.",
                CreatedBy = username,
                ActionDate = DateTime.Now
            };
            await _unitOfWork.LockerLogs.AddAsync(log);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
