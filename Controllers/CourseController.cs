using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace SIMS.Controllers
{
    [Authorize] // login required for all users
    public class CourseController : Controller
    {
        private readonly SIMSDbContext _context;
        public CourseController(SIMSDbContext context)
        {
            _context = context;
        }

        // All logged-in users can view courses
        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }

        public IActionResult Details(int id)
        {
            var course = _context.Courses.FirstOrDefault(m => m.Id == id);
            if (course == null) return NotFound();
            return View(course);
        }

        // === IMPORTANT CHANGE: Only Teachers can manage ===
        [Authorize(Roles = "Teacher")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult Create(string name, string description)
        {
            var course = new Course { Name = name, Description = description };
            _context.Courses.Add(course);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult Edit(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult Edit(int id, string name, string description)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();
            course.Name = name;
            course.Description = description;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Teacher")]
        public IActionResult DeleteConfirmed(int id)
        {
            var course = _context.Courses.Find(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}