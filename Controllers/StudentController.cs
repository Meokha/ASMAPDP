using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    // allow both Admin and Teacher roles to access this controller
    [Authorize(Roles = "Admin,Teacher")]
    public class StudentController : Controller
    {
        private readonly SIMSDbContext _context;
        public StudentController(SIMSDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students.Include(s => s.User).ToListAsync();
            return View(students);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string studentCode, string fullName, string email, string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
            }
            if (ModelState.ErrorCount == 0)
            {
                var user = new User
                {
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = "Student",
                    Email = email
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var student = new Student
                {
                    UserId = user.Id,
                    StudentCode = studentCode,
                    FullName = fullName,
                    Email = email
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["studentCode"] = studentCode;
            ViewData["fullName"] = fullName;
            ViewData["email"] = email;
            ViewData["username"] = username;
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string studentCode, string fullName, string email, string username)
        {
             var studentToUpdate = await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
             if (studentToUpdate == null) return NotFound();

             if (await _context.Users.AnyAsync(u => u.Username == username && u.Id != studentToUpdate.UserId))
             {
                 ModelState.AddModelError("Username", "Username already exists.");
             }
             if (ModelState.ErrorCount == 0)
             {
                 studentToUpdate.StudentCode = studentCode;
                 studentToUpdate.FullName = fullName;
                 studentToUpdate.Email = email;
                 if (studentToUpdate.User != null)
                 {
                     studentToUpdate.User.Username = username;
                     studentToUpdate.User.Email = email;
                 }
                 await _context.SaveChangesAsync();
                 return RedirectToAction(nameof(Index));
             }
             return View(studentToUpdate);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        // Action to handle the deletion of a student
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                var user = await _context.Users.FindAsync(student.UserId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}