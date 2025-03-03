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

        group.MapGet("/feedbacks/{id:guid}", async (IFeedbackService feedbackService, Guid id) =>
        {
            var result = await feedbackService.GetById(id);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedback")
        .WithSummary("Get feedback by id");

        group.MapGet("assignments/{assignmentId:guid}/feedbacks", async (IFeedbackService feedbackService, Guid assignmentId) =>
        {
            var result = await feedbackService.GetByAssignment(assignmentId);
            return result.MapToResponse(feedbacks => Results.Ok(feedbacks));
        })
        .Produces<IEnumerable<FeedbackResponse>>()
        .WithName("GetAllFeedbacksByAssignment")
        .WithSummary("Get all feedbacks by assignment id");

        group.MapGet("students/{studentId:guid}/assignments/{assignmentId:guid}/feedbacks", async (IFeedbackService feedbackService, Guid studentId, Guid assignmentId) =>
        {
            var result = await feedbackService.GetByStudentAssignment(studentId, assignmentId);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedbackByStudentAssignment")
        .WithSummary("Get feedback by student id and assignment id");

        group.MapGet("teams/{teamId:guid}/assignments/{assignmentId:guid}/feedbacks", async (IFeedbackService feedbackService, Guid teamId, Guid assignmentId) =>
        {
            var result = await feedbackService.GetByTeamAssignment(teamId, assignmentId);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedbackByTeamAssignment")
        .WithSummary("Get feedback by team id and assignment id");

        group.MapPost("feedbacks", async (IFeedbackService feedbackService, CreateFeedbackRequest request) =>
        {
            var result = await feedbackService.Create(request);
            return result.MapToResponse(feedback => Results.CreatedAtRoute
            (
                "GetFeedback",
                new { id = feedback.Id },
                feedback
            ));
        })
        .Produces<FeedbackResponse>()
        .WithName("CreateFeedback")
        .WithSummary("Create new feedback");

        group.MapPut("feedbacks/{id:guid}", async (IFeedbackService feedbackService, Guid id, UpdateFeedbackRequest request) =>
        {
            var result = await feedbackService.Update(request, id);
            return result.MapToResponse(feedback => Results.Ok(feedback));
        })
        .Produces<FeedbackResponse>()
        .WithName("UpdateFeedback")
        .WithSummary("Update feedback by id");

        group.MapDelete("feedbacks/{id:guid}", async (IFeedbackService feedbackService, Guid id) =>
        {
            var result = await feedbackService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteFeedback")
        .WithSummary("Delete feedback by id");
    }
}
