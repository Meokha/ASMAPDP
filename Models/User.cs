using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class User
    {
        public int Id { get; set; }
        
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; } // Admin hoặc Student

        public required string Email { get; set; }
    }
}
