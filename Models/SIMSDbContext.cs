using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

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
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = global::BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                Email = "admin@sims.local"
            });
        }
    }
}