using Microsoft.EntityFrameworkCore;
using BCrypt.Net; // <-- BƯỚC 1: THÊM DÒNG NÀY

namespace SIMS.Models
{
    public class SIMSDbContext : DbContext
    {
        public SIMSDbContext(DbContextOptions<SIMSDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BƯỚC 2: THAY THẾ TOÀN BỘ KHỐI CODE TẠO TÀI KHOẢN ADMIN
            
            // seed tài khoản Admin bằng BCrypt, an toàn và đồng bộ với hệ thống đăng nhập
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                // Dùng BCrypt.HashPassword để băm mật khẩu một cách an toàn
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                Email = "admin@sims.local"
            });
        }
    }
}