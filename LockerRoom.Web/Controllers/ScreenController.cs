using Microsoft.AspNetCore.Mvc;
using LockerRoom.Infrastructure.Data;
using System.Linq;

namespace LockerRoom.Web.Controllers
{
    public class ScreenController : Controller
    {
        private readonly MainDbContext _context;

        public ScreenController(MainDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["HideLayout"] = true; // ✅ يخفي النافبار
            return View();
        }

        [HttpGet]
        public IActionResult GetBookingByBracelet(string bracelet)
        {
            var booking = (from b in _context.Bookings
                           join l in _context.Lockers on b.LockerID equals l.LockerID
                           where b.BraceletCode == bracelet && b.IsActive
                           select new
                           {
                               groupId = l.GroupID,
                               lockerCode = l.LockerCode,
                               gender = l.Gender
                           }).FirstOrDefault();

            if (booking == null)
                return Json(new { success = false });

            return Json(booking);
        }
    }
}
