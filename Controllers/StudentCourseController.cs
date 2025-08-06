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

        // Action GET để hiển thị trang xếp lớp
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu cần thiết cho form và danh sách
            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Courses = await _context.Courses.ToListAsync();

            var studentCourses = await _context.StudentCourses
                .Include(sc => sc.Student)
                .Include(sc => sc.Course)
                .OrderBy(sc => sc.Student.FullName) // Sắp xếp cho dễ nhìn
                .ToListAsync();

            return View(studentCourses);
        }

        // Action POST để thực hiện việc gán
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Assign(int studentId, int courseId)
        {
            // Kiểm tra đầu vào
            if (studentId > 0 && courseId > 0)
            {
                // Kiểm tra xem bản ghi đã tồn tại chưa
                var existingAssignment = await _context.StudentCourses
                    .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

                if (existingAssignment == null)
                {
                    var studentCourse = new StudentCourse { StudentId = studentId, CourseId = courseId };
                    _context.StudentCourses.Add(studentCourse);
                    await _context.SaveChangesAsync();
                }
            }
            // Sau khi gán xong, quay lại trang Index để xem kết quả
            return RedirectToAction(nameof(Index));
        }

        // Action POST để thực hiện việc xóa
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

        // Action của sinh viên để tự đăng ký (giữ nguyên)
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

        // Action của sinh viên để xem khóa học của mình (giữ nguyên)
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
    // Tìm bản ghi cần sửa
    var assignment = await _context.StudentCourses
        .Include(sc => sc.Student)
        .Include(sc => sc.Course)
        .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);

    if (assignment == null)
    {
        return NotFound();
    }

    // Lấy danh sách để điền vào dropdown
    ViewBag.Students = await _context.Students.ToListAsync();
    ViewBag.Courses = await _context.Courses.ToListAsync();

    return View(assignment);
}

// ==========================================================
// BƯỚC 2.2: Action POST để nhận dữ liệu và lưu
// ==========================================================
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Teacher")]
public async Task<IActionResult> Edit(int originalStudentId, int originalCourseId, int newStudentId, int newCourseId)
{
    // Kiểm tra xem người dùng có chọn đủ không
    if (newStudentId <= 0 || newCourseId <= 0)
    {
        ModelState.AddModelError("", "Vui lòng chọn cả sinh viên và khóa học.");
        // Nếu có lỗi, phải load lại dữ liệu cho view
         ViewBag.Students = await _context.Students.ToListAsync();
         ViewBag.Courses = await _context.Courses.ToListAsync();
         // Lấy lại bản ghi gốc để hiển thị
         var originalAssignment = await _context.StudentCourses.FindAsync(originalStudentId, originalCourseId);
         return View(originalAssignment);
    }
    
    // Tìm bản ghi gốc cần cập nhật
    var assignmentToUpdate = await _context.StudentCourses.FindAsync(originalStudentId, originalCourseId);
    if (assignmentToUpdate == null)
    {
        return NotFound();
    }

    // Kiểm tra xem bản ghi mới đã tồn tại chưa (trừ chính nó)
    bool isNewAssignmentExist = await _context.StudentCourses
        .AnyAsync(sc => sc.StudentId == newStudentId && sc.CourseId == newCourseId);

    if (isNewAssignmentExist && (originalStudentId != newStudentId || originalCourseId != newCourseId))
    {
         ModelState.AddModelError("", "Bản ghi gán này đã tồn tại.");
    }

    if (ModelState.ErrorCount == 0)
    {
        // Nếu có sự thay đổi
        if (originalStudentId != newStudentId || originalCourseId != newCourseId)
        {
            // Xóa bản ghi cũ
            _context.StudentCourses.Remove(assignmentToUpdate);

            // Tạo bản ghi mới
            var newAssignment = new StudentCourse { StudentId = newStudentId, CourseId = newCourseId };
            _context.StudentCourses.Add(newAssignment);

            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
    
    // Nếu có lỗi, quay lại view Edit
    ViewBag.Students = await _context.Students.ToListAsync();
    ViewBag.Courses = await _context.Courses.ToListAsync();
    return View(assignmentToUpdate);
}
    }
}