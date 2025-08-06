using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using System.Linq;

namespace SIMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly SIMSDbContext _context;
        public CourseController(SIMSDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var courses = _context.Courses.ToList();
            return View(courses);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(string name, string description)
        {
            var course = new Course
            {
                Name = name,
                Description = description
            };
            _context.Courses.Add(course);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();
            return View(course);
        }
        [HttpPost]
        public IActionResult Edit(int id, string name, string description)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();
            course.Name = name;
            course.Description = description;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var course = _context.Courses.Find(id);
            if (course == null) return NotFound();
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
