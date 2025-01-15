using Api.Students.Contracts;
using Database.Models;

namespace Api.Students;

public static class StudentMapping
{
    public static IEnumerable<StudentResponse> MapToStudentResponse(this IEnumerable<User> users)
    {
        return users.Select(u => u.MapToStudentResponse());
    }

    public static StudentResponse MapToStudentResponse(this User user)
    {
        return new StudentResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
        };
    }
}
