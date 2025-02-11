using Api.Feedbacks.Contracts;

namespace Api.Feedbacks;

public static class FeedbackEndpoints
{
    public static void MapFeedbackEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Feedbacks");

        group.MapGet("feedbacks", async (IFeedbackService feedbackService) =>
        {
            var feedbacks = await feedbackService.GetAll();
            return Results.Ok(feedbacks);
        })
        .Produces<IEnumerable<FeedbackResponse>>()
        .WithName("GetAllFeedbacks")
        .WithSummary("Get all feedbacks");

        group.MapGet("/feedbacks/{id:guid}", async (IFeedbackService feedbackService, Guid id) =>
        {
            var feedback = await feedbackService.GetById(id);
            return feedback is not null ? Results.Ok(feedback) : Results.NotFound();
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedback")
        .WithSummary("Get feedback by id");

        group.MapGet("students/{studentId:guid}/assignments/{assignmentId:guid}/feedbacks", async (IFeedbackService feedbackService, Guid studentId, Guid assignmentId) =>
        {
            var result = await feedbackService.GetByStudentAssignment(studentId, assignmentId);
            return result.Match
            (
                feedback => feedback is not null ? Results.Ok(feedback) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<FeedbackResponse>()
        .WithName("GetFeedbackByStudentAssignment")
        .WithSummary("Get feedback by student id and assignment id");

        group.MapPost("feedbacks", async (IFeedbackService feedbackService, CreateFeedbackRequest request) =>
        {
            var result = await feedbackService.Create(request);

            return result.Match
            (
                feedback => feedback is not null ? Results.CreatedAtRoute
                (
                    "GetFeedback",
                    new { id = feedback.Id },
                    feedback
                ) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<FeedbackResponse>()
        .WithName("CreateFeedback")
        .WithSummary("Create new feedback");

        group.MapPut("feedbacks/{id:guid}", async (IFeedbackService feedbackService, Guid id, UpdateFeedbackRequest request) =>
        {
            var result = await feedbackService.Update(request, id);

            return result.Match
            (
                feedback => feedback is not null ? Results.Ok(feedback) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<FeedbackResponse>()
        .WithName("UpdateFeedback")
        .WithSummary("Update feedback by id");

        group.MapDelete("feedbacks/{id:guid}", async (IFeedbackService feedbackService, Guid id) =>
        {
            var deleted = await feedbackService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteFeedback")
        .WithSummary("Delete feedback by id");
    }
}
