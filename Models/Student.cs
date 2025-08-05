using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Student
    {
        public int Id { get; set; }
        public required string StudentCode { get; set; } = null!;
        public int UserId { get; set; }

        public required string FullName { get; set; } = default!; // <-- Sửa ở đây

        public required string Email { get; set; } = default!; // <-- Sửa ở đây

        // Thuộc tính điều hướng (Navigation Property)
        public User User { get; set; } = default!;
    }
}