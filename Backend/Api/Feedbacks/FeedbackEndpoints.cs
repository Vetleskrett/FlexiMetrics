using Api.Feedbacks.Contracts;

namespace Api.Feedbacks;

public static class FeedbackEndpoints
{
    public static void MapFeedbackEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Feedbacks").RequireAuthorization();

        group.MapGet("feedbacks", async (IFeedbackService feedbackService) =>
        {
            var result = await feedbackService.GetAll();
            return result.MapToResponse(feedbacks => Results.Ok(feedbacks));
        })
        .Produces<IEnumerable<FeedbackResponse>>()
        .WithName("GetAllFeedbacks")
        .WithSummary("Get all feedbacks");

        group.MapGet("/feedbacks/{feedbackId:guid}", async (IFeedbackService feedbackService, Guid feedbackId) =>
        {
            var result = await feedbackService.GetById(feedbackId);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedback")
        .RequireAuthorization("Feedback")
        .WithSummary("Get feedback by id");

        group.MapGet("assignments/{assignmentId:guid}/feedbacks", async (IFeedbackService feedbackService, Guid assignmentId) =>
        {
            var result = await feedbackService.GetByAssignment(assignmentId);
            return result.MapToResponse(feedbacks => Results.Ok(feedbacks));
        })
        .Produces<IEnumerable<FeedbackResponse>>()
        .WithName("GetAllFeedbacksByAssignment")
        .RequireAuthorization("TeacherForAssignment")
        .WithSummary("Get all feedbacks by assignment id");

        group.MapGet("students/{studentId:guid}/assignments/{assignmentId:guid}/feedbacks", async (IFeedbackService feedbackService, Guid studentId, Guid assignmentId) =>
        {
            var result = await feedbackService.GetByStudentAssignment(studentId, assignmentId);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedbackByStudentAssignment")
        .RequireAuthorization("TeacherForAssignmentOrStudent")
        .WithSummary("Get feedback by student id and assignment id");

        group.MapGet("teams/{teamId:guid}/assignments/{assignmentId:guid}/feedbacks", async (IFeedbackService feedbackService, Guid teamId, Guid assignmentId) =>
        {
            var result = await feedbackService.GetByTeamAssignment(teamId, assignmentId);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedbackByTeamAssignment")
        .RequireAuthorization("TeacherForAssignmentOrTeam")
        .WithSummary("Get feedback by team id and assignment id");

        group.MapPost("feedbacks", async (IFeedbackService feedbackService, CreateFeedbackRequest request) =>
        {
            var result = await feedbackService.Create(request);
            return result.MapToResponse(feedback => Results.CreatedAtRoute
            (
                "GetFeedback",
                new { feedbackId = feedback.Id },
                feedback
            ));
        })
        .Produces<FeedbackResponse>()
        .WithName("CreateFeedback")
        .RequireAuthorization("Teacher")
        .WithSummary("Create new feedback");

        group.MapPut("feedbacks/{feedbackId:guid}", async (IFeedbackService feedbackService, Guid feedbackId, UpdateFeedbackRequest request) =>
        {
            var result = await feedbackService.Update(request, feedbackId);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("UpdateFeedback")
        .RequireAuthorization("TeacherForFeedback")
        .WithSummary("Update feedback by id");

        group.MapDelete("feedbacks/{feedbackId:guid}", async (IFeedbackService feedbackService, Guid feedbackId) =>
        {
            var result = await feedbackService.DeleteById(feedbackId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteFeedback")
        .RequireAuthorization("TeacherForFeedback")
        .WithSummary("Delete feedback by id");
    }
}
