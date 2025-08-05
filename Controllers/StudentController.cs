using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SIMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly SIMSDbContext _context;
        public StudentController(SIMSDbContext context)
        {
            _context = context;
        }

        // Danh sách sinh viên
        public IActionResult Index()
        {
            var students = _context.Students.Include(s => s.User).ToList();
            return View(students);
        }

        // Thêm sinh viên
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
       public IActionResult Create(string studentCode, string fullName, string email, string username, string password)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username đã tồn tại");
                return View();
            }
            // Mã hóa mật khẩu
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "Student",
                Email = email
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            var student = new Student
            {
                UserId = user.Id,
                StudentCode = studentCode,
                FullName = fullName,
                Email = email
            };
            _context.Students.Add(student);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Sửa sinh viên
        public IActionResult Edit(int id)
        {
            var student = _context.Students.Include(s => s.User).FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }
        [HttpPost]
        public IActionResult Edit(int id, string studentCode, string fullName, string email, string username, string newPassword)
        {
            var student = _context.Students.Include(s => s.User).FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();
            student.StudentCode = studentCode;
            student.FullName = fullName;
            student.Email = email;
            if (student.User != null)
            {
                student.User.Username = username;
                student.User.Email = email;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Xóa sinh viên
        public IActionResult Delete(int id)
        {
            var student = _context.Students.Include(s => s.User).FirstOrDefault(s => s.Id == id);
            if (student == null) return NotFound();
            // Xóa cả user liên quan
            if (student.User != null)
            {
                _context.Users.Remove(student.User);
            }
            _context.Students.Remove(student);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
