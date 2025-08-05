using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public class Course
    {
        public int Id { get; set; }

        public required string Name { get; set; } = default!; // <-- Thêm = default!;

        // Khai báo là nullable (string?) vì nó là tùy chọn
        public string? Description { get; set; } // <-- Thêm dấu ?
    }
}