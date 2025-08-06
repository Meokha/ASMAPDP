using Microsoft.EntityFrameworkCore;
using SIMS.Models; // Đảm bảo bạn có using này

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Cấu hình DbContext
builder.Services.AddDbContext<SIMSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// =================================================================
// BẮT ĐẦU PHẦN CẦN THÊM VÀ SỬA
// =================================================================

// 2. Thêm dịch vụ Authentication và cấu hình sử dụng Cookies
//    Đây là phần quan trọng nhất để hệ thống [Authorize] hoạt động
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "MyCookieAuth";
        options.LoginPath = "/Account/Login"; // Chuyển đến trang Login nếu chưa đăng nhập
        options.AccessDeniedPath = "/Home/AccessDenied"; // Chuyển đến trang này nếu không có quyền
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian cookie hết hạn
    });

// 3. Thêm dịch vụ Authorization (không bắt buộc nhưng nên có để rõ ràng)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
    options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
});

// 4. Thêm và cấu hình dịch vụ Session
builder.Services.AddDistributedMemoryCache(); // Cần thiết cho Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =================================================================
// KẾT THÚC PHẦN CẦN THÊM VÀ SỬA
// =================================================================


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ===== THAY ĐỔI THỨ TỰ CÁC MIDDLEWARE =====
// Thứ tự này rất quan trọng để hệ thống hoạt động đúng
app.UseSession();           // Kích hoạt Session
app.UseAuthentication();    // Kích hoạt Authentication (Xác thực)
app.UseAuthorization();     // Kích hoạt Authorization (Phân quyền)


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // Sửa lại trang mặc định là Home/Index

app.Run();