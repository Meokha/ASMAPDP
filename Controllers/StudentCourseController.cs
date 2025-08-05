using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SIMS.Controllers
{
    public class StudentCourseController : Controller
    {
        private readonly SIMSDbContext _context;
        public StudentCourseController(SIMSDbContext context)
        {
            _context = context;
        }

        // Gán khóa học cho sinh viên
        public IActionResult Index()
        {
            var students = _context.Students.Include(s => s.User).ToList();
            var courses = _context.Courses.ToList();
            var studentCourses = _context.StudentCourses
                .Include(sc => sc.Student)
                .ThenInclude(s => s.User) // Thêm dòng này để load cả thông tin User của Student
                .Include(sc => sc.Course)
                .ToList();
            ViewBag.Students = students;
            ViewBag.Courses = courses;
            return View(studentCourses);
        }

        [HttpPost]
        public IActionResult Assign(int studentId, int courseId)
        {
            if (!_context.StudentCourses.Any(sc => sc.StudentId == studentId && sc.CourseId == courseId))
            {
                var sc = new StudentCourse { StudentId = studentId, CourseId = courseId };
                _context.StudentCourses.Add(sc);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // PHƯƠNG THỨC DELETE ĐÃ ĐƯỢC SỬA LỖI
        public IActionResult Delete(int studentId, int courseId)
        {
            // Tìm bản ghi dựa trên cả studentId và courseId
            var sc = _context.StudentCourses
                             .FirstOrDefault(sc => sc.StudentId == studentId && sc.CourseId == courseId);
            
            if (sc == null)
            {
                return NotFound();
            }

            _context.StudentCourses.Remove(sc);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Xem khóa học của sinh viên
        public IActionResult MyCourses()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _context.Students.FirstOrDefault(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound();
            }

            var courses = _context.StudentCourses
                .Where(sc => sc.StudentId == student.Id)
                .Include(sc => sc.Course)
                .Select(sc => sc.Course)
                .ToList();
            return View(courses);
        }
    }
}