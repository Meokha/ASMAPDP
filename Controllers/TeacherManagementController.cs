using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Models;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeacherManagementController : Controller
    {
        private readonly SIMSDbContext _context;

        public TeacherManagementController(SIMSDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teachers.Include(t => t.User).ToListAsync();
            return View(teachers);
        }

        public IActionResult Create()
        {
            return View();
        }
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(string fullName, string specialization, string username, string password, string email)
{
    if (string.IsNullOrEmpty(email))
    {
        ModelState.AddModelError("Email", "Vui lòng nhập địa chỉ email.");
    }
    if (await _context.Users.AnyAsync(u => u.Username == username))
    {
        ModelState.AddModelError("Username", "Name ");
    }
    if (ModelState.ErrorCount == 0)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            Role = "Teacher",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); 
        var teacher = new Teacher
        {
            FullName = fullName,
            Specialization = specialization,
            Email = email,
            UserId = user.Id
        };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
    ViewData["FullName"] = fullName;
    ViewData["Specialization"] = specialization;
    ViewData["Email"] = email;
    ViewData["Username"] = username;
    return View();
}

       public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }
    var teacher = await _context.Teachers
        .Include(t => t.User)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (teacher == null)
    {
        return NotFound();
    }

    return View(teacher);
}
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, string fullName, string specialization, string username, string email)
{
    var teacherToUpdate = await _context.Teachers
        .Include(t => t.User)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (teacherToUpdate == null)
    {
        return NotFound();
    }
    if (await _context.Users.AnyAsync(u => u.Username == username && u.Id != teacherToUpdate.UserId))
    {
        ModelState.AddModelError("Username", "This username is already in use.");
    }
    if (ModelState.ErrorCount == 0)
    {
        // Update the properties of the object retrieved from the DB
        teacherToUpdate.FullName = fullName;
        teacherToUpdate.Specialization = specialization;
        teacherToUpdate.Email = email; // Update the email of the Teacher

        // Update the information of the related User
        if (teacherToUpdate.User != null)
        {
            teacherToUpdate.User.Username = username;
            teacherToUpdate.User.Email = email;
        }

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError("", "Unable to save changes. The data may have been modified by another user.");
        }
    }
    return View(teacherToUpdate);
}

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var teacher = await _context.Teachers.Include(t => t.User).FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                var user = await _context.Users.FindAsync(teacher.UserId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                }
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}