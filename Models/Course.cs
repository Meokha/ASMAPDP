using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Course
    {
        public int Id { get; set; }
        public required string Name { get; set; } = default!;
        public string? Description { get; set; } = default!;
    }
}