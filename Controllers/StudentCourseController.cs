using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    [Authorize]
    public class StudentCourseController : Controller
    {
        private readonly SIMSDbContext _context;
        public StudentCourseController(SIMSDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Courses = await _context.Courses.ToListAsync();

            var studentCourses = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .OrderBy(sc => sc.Student.FullName)
                .ToListAsync();

            return View(studentCourses);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Assign(int studentId, int courseId)
        {
            if (studentId > 0 && courseId > 0)
            {
                // check if the assignment already exists
                var existingAssignment = await _context.StudentCourses
                    .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

                if (existingAssignment == null)
                {
                    var studentCourse = new StudentCourse { StudentId = studentId, CourseId = courseId };
                    _context.StudentCourses.Add(studentCourse);
                    await _context.SaveChangesAsync();
                }
            }
            // After assigning, redirect back to the Index page to see the results
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int studentId, int courseId)
        {
            var assignmentToDelete = await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

            if (assignmentToDelete != null)
            {
                _context.StudentCourses.Remove(assignmentToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound("Không tìm thấy thông tin sinh viên.");

            bool isEnrolled = await _context.StudentCourses.AnyAsync(sc => sc.StudentId == student.Id && sc.CourseId == courseId);
            if (!isEnrolled)
            {
                _context.StudentCourses.Add(new StudentCourse { StudentId = student.Id, CourseId = courseId });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký khóa học thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Bạn đã ở trong khóa học này.";
            }

            return RedirectToAction("Details", "Teacher", new { id = courseId });
        }
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyCourses()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null) return NotFound();

            var courses = await _context.StudentCourses
                .Where(sc => sc.StudentId == student.Id)
                .Include(sc => sc.Course)
                .Select(sc => sc.Course)
                .ToListAsync();

            return View(courses);
        }
        [Authorize(Roles = "Teacher")]
public async Task<IActionResult> Edit(int studentId, int courseId)
{
    var assignment = await _context.StudentCourses
        .Include(sc => sc.Student)
        .Include(sc => sc.Course)
        .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

    if (assignment == null)
    {
        return NotFound();
    }
    ViewBag.Students = await _context.Students.ToListAsync();
    ViewBag.Courses = await _context.Courses.ToListAsync();

    return View(assignment);
}
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Teacher")]
public async Task<IActionResult> Edit(int originalStudentId, int originalCourseId, int newStudentId, int newCourseId)
{
    if (newStudentId <= 0 || newCourseId <= 0)
    {
        ModelState.AddModelError("", "Vui lòng chọn cả sinh viên và khóa học.");
         ViewBag.Students = await _context.Students.ToListAsync();
         ViewBag.Courses = await _context.Courses.ToListAsync();
         var originalAssignment = await _context.StudentCourses.FindAsync(originalStudentId, originalCourseId);
         return View(originalAssignment);
    }
    var assignmentToUpdate = await _context.StudentCourses.FindAsync(originalStudentId, originalCourseId);
    if (assignmentToUpdate == null)
    {
        return NotFound();
    }
    bool isNewAssignmentExist = await _context.StudentCourses
        .AnyAsync(sc => sc.StudentId == newStudentId && sc.CourseId == newCourseId);

    if (isNewAssignmentExist && (originalStudentId != newStudentId || originalCourseId != newCourseId))
    {
         ModelState.AddModelError("", "Bản ghi gán này đã tồn tại.");
    }

    if (ModelState.ErrorCount == 0)
    {
        if (originalStudentId != newStudentId || originalCourseId != newCourseId)
        {
            _context.StudentCourses.Remove(assignmentToUpdate);

            var newAssignment = new StudentCourse { StudentId = newStudentId, CourseId = newCourseId };
            _context.StudentCourses.Add(newAssignment);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    ViewBag.Students = await _context.Students.ToListAsync();
    ViewBag.Courses = await _context.Courses.ToListAsync();
    return View(assignmentToUpdate);
}
    }
}