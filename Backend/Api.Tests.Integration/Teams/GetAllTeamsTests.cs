using Database.Models;

namespace Api.Tests.Integration.Teams;

public class GetAllTeamsTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllTeams_ShouldReturnEmpty_WhenEmpty()
    {
        var response = await Client.GetAsync("teams");
        await Verify(response);
    }

    [Fact]
    public async Task GetAllTeams_ShouldReturnEmptyStudents_WhenNoStudents()
    {
        var course = ModelFactory.CreateCourse();

        ModelFactory.CreateTeams(course.Id, 3);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"teams");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllTeams_ShouldReturnTeams_WhenTeamsExists()
    {
        var courses = ModelFactory.CreateCourses(2);

        var students = ModelFactory.CreateStudents(6);

        foreach (var course in courses)
        {
            foreach (var student in students)
            {
                ModelFactory.CreateCourseStudent(course.Id, student.Id);
            }
        }

        ModelFactory.CreateTeamsWithStudents(courses[0].Id, students, 3);
        ModelFactory.CreateTeamsWithStudents(courses[1].Id, students, 2);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync("teams");

        await Verify(response);
    }
}