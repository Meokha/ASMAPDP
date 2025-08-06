using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    [PrimaryKey(nameof(StudentId), nameof(CourseId))]
    public class StudentCourse
    {
        public int StudentId { get; set; }  
        public int CourseId { get; set; }
        public Student Student { get; set; } = default!;
        public Course Course { get; set; } = default!;
    }
}