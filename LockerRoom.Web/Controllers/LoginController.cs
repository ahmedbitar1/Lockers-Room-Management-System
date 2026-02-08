using Microsoft.AspNetCore.Mvc;
using LockerRoom.Infrastructure.Data;
using LockerRoom.Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
 
namespace LockerRoom.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly MainDbContext _context;

        public LoginController(MainDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Please enter both username and password.";
                return View(model);
            }

            string hashedPassword = ComputeSha256Hash(model.Password);

            var user = _context.Users.FirstOrDefault(u =>
                u.Username == model.Username && u.PasswordHash == hashedPassword);


            if (user != null)
            {
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role ?? "");

                if (user.Role != null && user.Role.Equals("screen", StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Screen");
                }
                else
                {
                    return RedirectToAction("Index", "Reception");
                }
            }

            ViewBag.Error = "Invalid username or password.";
            return View(model);
        }

        public IActionResult Logout()
        {  
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                
                byte[] bytes = sha256Hash.ComputeHash(Encoding.Default.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

}
