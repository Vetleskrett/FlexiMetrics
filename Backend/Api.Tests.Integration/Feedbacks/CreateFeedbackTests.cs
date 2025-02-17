using Api.Feedbacks.Contracts;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace Api.Tests.Integration.Feedbacks;

public class CreateFeedbackTests(ApiFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateFeedback_ShouldCreateNoGradingFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.NoGrading,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<Feedback>()
            .AnyAsync(f =>
                f.AssignmentId == request.AssignmentId &&
                f.StudentId == request.StudentId &&
                f.Comment == request.Comment
            )
        );
    }

    [Fact]
    public async Task CreateFeedback_ShouldCreateApprovalFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.ApprovalGrading,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Comment = "Looks good to me",
            IsApproved = true,
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<ApprovalFeedback>()
            .AnyAsync(f =>
                f.AssignmentId == request.AssignmentId &&
                f.StudentId == request.StudentId &&
                f.Comment == request.Comment &&
                f.IsApproved == request.IsApproved
            )
        );
    }

    [Fact]
    public async Task CreateFeedback_ShouldCreateLetterFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.LetterGrading,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Comment = "Looks good to me",
            LetterGrade = LetterGrade.C
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<LetterFeedback>()
            .AnyAsync(f =>
                f.AssignmentId == request.AssignmentId &&
                f.StudentId == request.StudentId &&
                f.Comment == request.Comment &&
                f.LetterGrade == request.LetterGrade
            )
        );
    }

    [Fact]
    public async Task CreateFeedback_ShouldCreatePointsFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.PointsGrading,
            maxPoints: 100,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Comment = "Looks good to me",
            Points = 75
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<PointsFeedback>()
            .AnyAsync(f =>
                f.AssignmentId == request.AssignmentId &&
                f.StudentId == request.StudentId &&
                f.Comment == request.Comment &&
                f.Points == request.Points
            )
        );
    }

    [Fact]
    public async Task CreateFeedback_ShouldCreateTeamFeedback_WhenValidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var team = ModelFactory.CreateTeam(course.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            collaboration: CollaborationType.Teams,
            gradingType: GradingType.NoGrading,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            TeamId = team.Id,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
        Assert.True(await DbContext.Feedbacks
            .OfType<Feedback>()
            .AnyAsync(f =>
                f.AssignmentId == request.AssignmentId &&
                f.TeamId == request.TeamId &&
                f.Comment == request.Comment
            )
        );
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnBadRequest_WhenInvalidRequest()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.LetterGrading,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Comment = "Looks good to me",
            IsApproved = true
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnBadRequest_WhenDueDateNotPassed()
    {
        var course = ModelFactory.CreateCourse();
        var student = ModelFactory.CreateStudent();
        ModelFactory.CreateCourseStudent(course.Id, student.Id);
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.NoGrading,
            offset: TimeSpan.FromDays(7)
        );
        await DbContext.SaveChangesAsync();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            StudentId = student.Id,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnNotFound_WhenInvalidAssignment()
    {
        var student = ModelFactory.CreateStudent();
        await DbContext.SaveChangesAsync();
        var assignmentId = Guid.NewGuid();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignmentId,
            StudentId = student.Id,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnNotFound_WhenInvalidStudent()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            gradingType: GradingType.NoGrading,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();
        var studentId = Guid.NewGuid();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            StudentId = studentId,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }

    [Fact]
    public async Task CreateFeedback_ShouldReturnNotFound_WhenInvalidTeam()
    {
        var course = ModelFactory.CreateCourse();
        var assignment = ModelFactory.CreateAssignment(
            course.Id,
            collaboration: CollaborationType.Teams,
            gradingType: GradingType.NoGrading,
            offset: TimeSpan.FromDays(-7)
        );
        await DbContext.SaveChangesAsync();
        var teamId = Guid.NewGuid();

        var request = new CreateFeedbackRequest
        {
            AssignmentId = assignment.Id,
            TeamId = teamId,
            Comment = "Looks good to me"
        };

        var response = await Client.PostAsJsonAsync("feedbacks", request);

        await Verify(response);
    }
}