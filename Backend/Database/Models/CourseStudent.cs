using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[PrimaryKey(nameof(CourseId), nameof(StudentId))]
public class CourseStudent
{
    public required Guid CourseId { get; set; }
    public Course? Course { get; set; }
    public required Guid StudentId { get; set; }
    public User? Student { get; set; }
}