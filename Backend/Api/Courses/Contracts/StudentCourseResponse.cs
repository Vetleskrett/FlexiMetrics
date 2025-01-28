using Api.Teachers.Contracts;
using Api.Teams.Contracts;
using Database.Models;

namespace Api.Courses.Contracts;

public class StudentCourseResponse
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required int Year { get; init; }
    public required Semester Semester { get; init; }
    public required TeamResponse? Team { get; init; }
    public required List<TeacherResponse> Teachers { get; init; }
}
