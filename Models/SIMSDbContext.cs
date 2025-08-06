using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace SIMS.Models
{
    public class SIMSDbContext : DbContext
    {
        public SIMSDbContext(DbContextOptions<SIMSDbContext> options) : base(options) { }

        // Bỏ constructor rỗng không cần thiết đi để code sạch hơn
        // public SIMSDbContext() { } 

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- DỮ LIỆU MẪU MỚI VỚI 3 VAI TRÒ ---

            // 1. Tạo các User với các Role đã được phân lại
            var superAdminUser = new User 
            { 
                Id = 1, 
                Username = "admin", 
                Email = "admin@sims.local", 
                Role = "Admin", // Admin mới, quyền lực nhất
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("admin123") 
            };
            
            var teacherUser = new User 
            { 
                Id = 2, 
                Username = "teacher", 
                Email = "teacher1@sims.local", 
                Role = "Teacher", // Vai trò giáo viên
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("teacher123") 
            };
            
            var studentUser = new User 
            { 
                Id = 3, 
                Username = "student", 
                Email = "student1@sims.local", 
                Role = "Student", // Vai trò sinh viên
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("student123")
            };

            modelBuilder.Entity<User>().HasData(superAdminUser, teacherUser, studentUser);

            // 2. Tạo thông tin Teacher tương ứng với teacherUser
            // Lưu ý: Không có bản ghi Teacher/Student nào cho superAdminUser vì họ chỉ là quản trị viên hệ thống.
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { 
                    Id = 1, 
                    FullName = "Teacher One", 
                    Email = "teacher1@sims.local", 
                    Specialization = "Mathematics", 
                    UserId = teacherUser.Id 
                }
            );

            // 3. Tạo thông tin Student tương ứng với studentUser
            modelBuilder.Entity<Student>().HasData(
                new Student { 
                    Id = 1, 
                    FullName = "Student One", 
                    Email = "student1@sims.local", 
                    StudentCode = "SV001", 
                    UserId = studentUser.Id 
                }
            );

            // Thiết lập mối quan hệ một-nhiều giữa User và Student/Teacher (tùy chọn nhưng khuyến khích)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne() // Nếu một User chỉ có thể là một Student
                .HasForeignKey<Student>(s => s.UserId);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne() // Nếu một User chỉ có thể là một Teacher
                .HasForeignKey<Teacher>(t => t.UserId);
        }
    }
}