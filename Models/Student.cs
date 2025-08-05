using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Student
    {
        public int Id { get; set; }
        public required string StudentCode { get; set; } = null!;
        public int UserId { get; set; }

        public required string FullName { get; set; } = default!;

        public required string Email { get; set; } = default!; 

        public User User { get; set; } = default!;
    }
}