using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    // Đánh dấu đây là một bảng nối với khóa chính được tạo từ 2 cột
    [PrimaryKey(nameof(StudentId), nameof(CourseId))]
    public class StudentCourse
    {
        // Foreign Key cho Student
        public int StudentId { get; set; }
        
        // Foreign Key cho Course
        public int CourseId { get; set; }

        // Navigation property đến Student
        public Student Student { get; set; } = default!;

        // Navigation property đến Course
        public Course Course { get; set; } = default!;
    }
}