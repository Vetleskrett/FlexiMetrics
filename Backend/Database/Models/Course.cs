using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Course
{
    [Key]
    public required Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required int Year { get; set; }
    public required Semester Semester { get; set; }
    // public List<User>? Students { get; set; }
    // Replace with Seperate CourseStudent, like CourseTeacher
    public List<Team>? Teams { get; set; }
}
