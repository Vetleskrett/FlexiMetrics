namespace Api.Tests.Integration.Students;

public class GetStudentTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetStudent_ShouldReturnStudent_WhenStudentExists()
    {
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}");

        await Verify(response);
    }

    [Fact]
    public async Task GetStudent_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var id = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{id}");

        await Verify(response);
    }
}
