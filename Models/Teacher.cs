namespace SIMS.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? Specialization { get; set; }

        // Foreign Key to User
        public int UserId { get; set; }
        public virtual User User { get; set; } = default!;
    }
}