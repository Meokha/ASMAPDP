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

       // Action POST để nhận dữ liệu và lưu
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(string fullName, string specialization, string username, string password, string email)
{
    // Kiểm tra các điều kiện custom trước
    if (string.IsNullOrEmpty(email))
    {
        ModelState.AddModelError("Email", "Vui lòng nhập địa chỉ email.");
    }
    if (await _context.Users.AnyAsync(u => u.Username == username))
    {
        ModelState.AddModelError("Username", "Tên đăng nhập này đã được sử dụng.");
    }

    // Kiểm tra xem có lỗi nào không
    if (ModelState.ErrorCount == 0)
    {
        // 1. Tạo đối tượng User trước
        var user = new User
        {
            Username = username,
            Email = email,
            Role = "Teacher",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // Lưu để User có ID

        // 2. Tạo đối tượng Teacher và gán User ID vừa tạo
        var teacher = new Teacher
        {
            FullName = fullName,
            Specialization = specialization,
            Email = email, // Đảm bảo Email được gán giá trị
            UserId = user.Id
        };
        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Nếu có lỗi, quay lại view Create để hiển thị lỗi
    // Ta truyền lại các giá trị đã nhập để người dùng không phải gõ lại
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

    // Lấy thông tin Teacher và User liên quan để hiển thị lên form
    var teacher = await _context.Teachers
        .Include(t => t.User)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (teacher == null)
    {
        return NotFound();
    }

    return View(teacher);
}

// Action POST: Nhận dữ liệu mới và lưu
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, string fullName, string specialization, string username, string email)
{
    // Tìm đối tượng Teacher và User gốc trong database
    var teacherToUpdate = await _context.Teachers
        .Include(t => t.User)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (teacherToUpdate == null)
    {
        return NotFound();
    }

    // Kiểm tra xem username mới có bị trùng với người khác không
    if (await _context.Users.AnyAsync(u => u.Username == username && u.Id != teacherToUpdate.UserId))
    {
        ModelState.AddModelError("Username", "Tên đăng nhập này đã được người khác sử dụng.");
    }

    // Nếu không có lỗi nào, tiến hành cập nhật
    if (ModelState.ErrorCount == 0)
    {
        // Cập nhật các thuộc tính của đối tượng đã lấy từ DB
        teacherToUpdate.FullName = fullName;
        teacherToUpdate.Specialization = specialization;
        teacherToUpdate.Email = email; // Cập nhật cả email của Teacher
        
        // Cập nhật cả thông tin của User liên quan
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
            // Xử lý lỗi nếu có ai đó đã xóa bản ghi này trong lúc mình đang sửa
            ModelState.AddModelError("", "Không thể lưu thay đổi. Dữ liệu có thể đã bị người khác thay đổi.");
        }
    }

    // Nếu có lỗi, trả về lại view Edit, các thông báo lỗi sẽ được hiển thị
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