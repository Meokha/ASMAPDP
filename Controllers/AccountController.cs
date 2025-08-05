using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using System.Linq;
using BCrypt.Net;

namespace SIMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly SIMSDbContext _context;

        public AccountController(SIMSDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string fullName, string email)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username đã tồn tại");
                return View();
            }

            var user = new User
            {
                Username = username,
                // DÒNG MỚI
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Student",
                Email = email
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var student = new Student
            {
                UserId = user.Id,
                FullName = fullName,
                Email = email,
                StudentCode = ""
            };
            _context.Students.Add(student);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);

            // DÒNG MỚI
        if (user == null || !global::BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Wrong Username or password");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}