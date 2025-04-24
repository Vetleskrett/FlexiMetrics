using Database.Models;
using FileStorage;
using Microsoft.EntityFrameworkCore;
using SeedingService.Courses;

namespace SeedingService;

public static class Seed
{
    // Temporary static Ids for the teacher and student users used in the frontend
    private static readonly Guid MAIN_TEACHER_ID = new("395c5222-f78e-687f-a744-6d4285c04a61");
    private static readonly Guid MAIN_STUDENT_ID = new("34b59962-367e-de04-fd91-a68d1d680839");

    public static async Task SeedDatabaseAsync(this DbContext dbContext, IFileStorage fileStorage)
    {
        fileStorage.DeleteAll();
        await dbContext.Set<Course>().ExecuteDeleteAsync();
        await dbContext.Set<User>().ExecuteDeleteAsync();

        var teachers = TestUsers.FEIDE_TEST_TEACHERS
            .Select(u => new User
            {
                Id = Guid.NewGuid(),
                Name = u.Name,
                Email = u.Username,
                Role = Role.Teacher
            }).ToList();

        teachers[0].Id = MAIN_TEACHER_ID;
        dbContext.AddRange(teachers);

        var students = TestUsers.FEIDE_TEST_STUDENTS
            .Select(u => new User
            {
                Id = Guid.NewGuid(),
                Name = u.Name,
                Email = u.Username,
                Role = Role.Student
            }).ToList();

        students[0].Id = MAIN_STUDENT_ID;
        dbContext.AddRange(students);

        await dbContext.SaveChangesAsync();

        await WebDevelopment.Seed(dbContext, fileStorage, teachers, students);
    }
}
