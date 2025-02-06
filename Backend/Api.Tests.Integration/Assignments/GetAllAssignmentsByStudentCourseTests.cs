using Database.Models;

namespace Api.Tests.Integration.Assignments;

public class GetAllAssignmentsByStudentCourseTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnEmpty_WhenEmpty()
    {
        var course = ModelFactory.GetValidCourse();
        var otherCourse = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        DbContext.Courses.Add(otherCourse);

        DbContext.Assignments.AddRange([
            ModelFactory.GetValidAssignment(otherCourse.Id),
            ModelFactory.GetValidAssignment(otherCourse.Id),
            ModelFactory.GetValidAssignment(otherCourse.Id)
        ]);

        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);
        DbContext.CourseStudents.Add(ModelFactory.GetValidCourseStudent(course.Id, student.Id));

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnAssignments_WhenAssignmentsExists()
    {
        var course = ModelFactory.GetValidCourse();
        var otherCourse = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        DbContext.Courses.Add(otherCourse);

        DbContext.Assignments.AddRange([
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id),
            ModelFactory.GetValidAssignment(course.Id),

            ModelFactory.GetValidAssignment(otherCourse.Id),
            ModelFactory.GetValidAssignment(otherCourse.Id)
        ]);

        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);
        DbContext.CourseStudents.Add(ModelFactory.GetValidCourseStudent(course.Id, student.Id));

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldOnlyReturnPublishedAssignments()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        List<Assignment> assignments = [
            ModelFactory.GetValidAssignment(course.Id, true, TimeSpan.FromDays(0)),
            ModelFactory.GetValidAssignment(course.Id, true, TimeSpan.FromDays(1)),
            ModelFactory.GetValidAssignment(course.Id, true, TimeSpan.FromDays(2)),

            ModelFactory.GetValidAssignment(course.Id, false, TimeSpan.FromDays(3)),
            ModelFactory.GetValidAssignment(course.Id, false, TimeSpan.FromDays(4)),
        ];
        DbContext.Assignments.AddRange(assignments);

        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);
        DbContext.CourseStudents.Add(ModelFactory.GetValidCourseStudent(course.Id, student.Id));

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldHaveIsDeliveredTrue_WhenDeliveryExists()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        List<Assignment> assignments = [
            ModelFactory.GetValidAssignment(course.Id, offset: TimeSpan.FromDays(0)),
            ModelFactory.GetValidAssignment(course.Id, offset: TimeSpan.FromDays(1)),
            ModelFactory.GetValidAssignment(course.Id, offset: TimeSpan.FromDays(2)),
            ModelFactory.GetValidAssignment(course.Id, offset: TimeSpan.FromDays(3)),
            ModelFactory.GetValidAssignment(course.Id, offset: TimeSpan.FromDays(4)),
        ];
        DbContext.Assignments.AddRange(assignments);

        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);
        DbContext.CourseStudents.Add(ModelFactory.GetValidCourseStudent(course.Id, student.Id));

        DbContext.Deliveries.AddRange([
            ModelFactory.GetValidStudentDelivery(assignments[0].Id, student.Id),
            ModelFactory.GetValidStudentDelivery(assignments[1].Id, student.Id),
            ModelFactory.GetValidStudentDelivery(assignments[2].Id, student.Id),
        ]);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);
        await DbContext.SaveChangesAsync();

        var studentId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{studentId}/course/{course.Id}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnNotFound_WhenInvalidCourse()
    {
        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);
        await DbContext.SaveChangesAsync();

        var courseId = Guid.NewGuid();

        var response = await Client.GetAsync($"students/{student.Id}/course/{courseId}/assignments");

        await Verify(response);
    }

    [Fact]
    public async Task GetAllAssignmentsByStudentCourse_ShouldReturnBadRequest_WhenStudentNotInCourse()
    {
        var student = ModelFactory.GetValidStudent();
        DbContext.Users.Add(student);

        var course = ModelFactory.GetValidCourse();
        DbContext.Courses.Add(course);

        await DbContext.SaveChangesAsync();

        var response = await Client.GetAsync($"students/{student.Id}/course/{course.Id}/assignments");

        await Verify(response);
    }
}