using Api.Teachers.Contracts;
using Database.Models;

namespace Api.Teachers;

public static class TeacherMapping
{
    public static IEnumerable<TeacherResponse> MapToTeacherResponse(this IEnumerable<User> users)
    {
        return users.Select(u => u.MapToTeacherResponse());
    }

    public static TeacherResponse MapToTeacherResponse(this User user)
    {
        return new TeacherResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
        };
    }
}
