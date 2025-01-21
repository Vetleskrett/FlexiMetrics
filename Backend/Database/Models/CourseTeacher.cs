using Microsoft.EntityFrameworkCore;

namespace Database.Models;

[PrimaryKey(nameof(CourseId), nameof(TeacherId))]
public class CourseTeacher
{
    public required Guid CourseId { get; set; }
    public Course? Course { get; set; }
    public required Guid TeacherId { get; set; }
    public User? Teacher { get; set; }
}