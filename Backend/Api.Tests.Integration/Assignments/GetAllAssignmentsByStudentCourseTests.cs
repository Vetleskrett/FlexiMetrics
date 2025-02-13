namespace Api.Tests.Integration.Assignments;

public class GetAllAssignmentsByStudentCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.CreateCourse();
        var otherCourse = ModelFactory.CreateCourse();
        ModelFactory.CreateAssignments(otherCourse.Id, 3);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnAssignments_WhenAssignmentsExists()
    {
        var course = ModelFactory.CreateCourse();
        var otherCourse = ModelFactory.CreateCourse();
        ModelFactory.CreateAssignments(course.Id, 3);
        ModelFactory.CreateAssignments(otherCourse.Id, 2);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldOnlyReturnPublishedAssignments()
    {
        var course = ModelFactory.CreateCourse();
        ModelFactory.CreateAssignments(course.Id, 3, true);
        ModelFactory.CreateAssignments(course.Id, 2, false);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldHaveIsDeliveredTrue_WhenDeliveryExists()
    {
        var course = ModelFactory.CreateCourse();
        var assignments = ModelFactory.CreateAssignments(course.Id, 5);
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        ModelFactory.CreateStudentDelivery(assignments[0].Id, student.Id);
        ModelFactory.CreateStudentDelivery(assignments[1].Id, student.Id);
        ModelFactory.CreateStudentDelivery(assignments[2].Id, student.Id);
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();
        var studentId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{studentId}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();
        var courseId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{student.Id}/course/{courseId}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnBadRequest_WhenStudentNotInCourse()
    {
        var student = ModelFactory.CreateStudent();
        var course = ModelFactory.CreateCourse();
        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }
}