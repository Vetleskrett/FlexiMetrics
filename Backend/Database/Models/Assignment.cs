using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class Assignment
    {
        [Key]
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime? DueDate { get; set; }
        public required Guid CourseId { get; set; }
    }
}
