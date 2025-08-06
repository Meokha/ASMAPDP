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
            var superAdminUser = new User 
            { 
                Id = 1, 
                Username = "admin", 
                Email = "admin@sims.local", 
                Role = "Admin", 
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("admin123") 
            };
            
            var teacherUser = new User 
            { 
                Id = 2, 
                Username = "teacher", 
                Email = "teacher1@sims.local", 
                Role = "Teacher", 
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("teacher123") 
            };
            
            var studentUser = new User 
            { 
                Id = 3, 
                Username = "student", 
                Email = "student1@sims.local", 
                Role = "Student",
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("student123")
            };

            modelBuilder.Entity<User>().HasData(superAdminUser, teacherUser, studentUser);
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { 
                    Id = 1, 
                    FullName = "Teacher One", 
                    Email = "teacher1@sims.local", 
                    Specialization = "Mathematics", 
                    UserId = teacherUser.Id 
                }
            );
            modelBuilder.Entity<Student>().HasData(
                new Student { 
                    Id = 1, 
                    FullName = "Student One", 
                    Email = "student1@sims.local", 
                    StudentCode = "SV001", 
                    UserId = studentUser.Id 
                }
            );
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne() 
                .HasForeignKey<Student>(s => s.UserId);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.UserId);
        }
    }
}