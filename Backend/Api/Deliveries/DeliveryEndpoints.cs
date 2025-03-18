using Api.Deliveries.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Deliveries;

public static class DeliveryEndpoints
{
    public static void MapDeliveryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Deliveries").RequireAuthorization();

        group.MapGet("deliveries", async (IDeliveryService deliveryService) =>
        {
            var result = await deliveryService.GetAll();
            return result.MapToResponse(deliveries => Results.Ok(deliveries));
        })
        .Produces<IEnumerable<DeliveryResponse>>()
        .WithName("GetAllDeliveries")
        .WithSummary("Get all deliveries");

        group.MapGet("deliveries/{id:guid}", async (IDeliveryService deliveryService, Guid id) =>
        {
            var result = await deliveryService.GetById(id);
            return result.MapToResponse(delivery => Results.Ok(delivery));
        })
        .Produces<DeliveryResponse>()
        .WithName("GetDelivery")
        .RequireAuthorization("InDelivery")
        .WithSummary("Get delivery by id");

        group.MapGet("students/{studentId:guid}/assignments/{assignmentId:guid}/deliveries",
            async (IDeliveryService deliveryService, Guid studentId, Guid assignmentId) =>
        {
            var result = await deliveryService.GetByStudentAssignment(studentId, assignmentId);
            return result.MapToResponse(delivery => Results.Ok(delivery));
        })
        .Produces<DeliveryResponse>()
        .WithName("GetDeliveryByStudentAssignment")
        .RequireAuthorization("TeacherForAssignmentOrStudent")
        .WithSummary("Get delivery by student id and assignment id");

        group.MapGet("teams/{teamId:guid}/assignments/{assignmentId:guid}/deliveries",
            async (IDeliveryService deliveryService, Guid teamId, Guid assignmentId) =>
        {
            var result = await deliveryService.GetByTeamAssignment(teamId, assignmentId);
            return result.MapToResponse(delivery => Results.Ok(delivery));
        })
        .Produces<DeliveryResponse>()
        .WithName("GetDeliveryByTeamAssignment")
        .RequireAuthorization("TeacherForAssignmentOrTeam")
        .WithSummary("Get delivery by team id and assignment id");

        group.MapGet("assignments/{assignmentId:guid}/deliveries", async (IDeliveryService deliveryService, Guid assignmentId) =>
        {
            var result = await deliveryService.GetAllByAssignment(assignmentId);
            return result.MapToResponse(deliveries => Results.Ok(deliveries));
        })
        .Produces<IEnumerable<DeliveryResponse>>()
        .WithName("GetAllDeliveriesByAssignment")
        .RequireAuthorization("TeacherForAssignment")
        .WithSummary("Get all deliveries by assignment id");

        group.MapPost("deliveries", async (IDeliveryService deliveryService, CreateDeliveryRequest request) =>
        {
            var result = await deliveryService.Create(request);
            return result.MapToResponse(delivery => Results.CreatedAtRoute
            (
                "GetDelivery",
                new { id = delivery.Id },
                delivery
            ));
        })
        .Produces<DeliveryResponse>()
        .WithName("CreateDelivery")
        .WithSummary("Create new delivery");

        group.MapPut("deliveries/{deliveryId}", async (IDeliveryService deliveryService, Guid deliveryId, UpdateDeliveryRequest request) =>
        {
            var result = await deliveryService.Update(request, deliveryId);
            return result.MapToResponse(delivery => Results.Ok(delivery));
        })
        .Produces<DeliveryResponse>()
        .WithName("UpdateDelivery")
        .RequireAuthorization("InDelivery")
        .WithSummary("Update delivery by id");

        group.MapGet("delivery-fields/{deliveryFieldId:guid}", async (IDeliveryService deliveryService, Guid deliveryFieldId) =>
        {
            var result = await deliveryService.DownloadFile(deliveryFieldId);
            return result.MapToResponse(file => Results.File(file.Stream, file.Metadata.ContentType, file.Metadata.FileName));
        })
        .Produces<FileStreamHttpResult>()
        .WithName("DownloadDeliveryFile")
        .RequireAuthorization("InDeliveryField")
        .WithSummary("Download delivery file");

        group.MapPost("delivery-fields/{deliveryFieldId:guid}", async (IDeliveryService deliveryService, IFormFile file, Guid deliveryFieldId) =>
        {
            var result = await deliveryService.UploadFile(file, deliveryFieldId);
            return result.MapToResponse(() => Results.Ok());
        })
        .Accepts<IFormFile>("multipart/form-data")
        .DisableAntiforgery()
        .WithName("UploadDeliveryFile")
        .RequireAuthorization("InDeliveryField")
        .WithSummary("Upload delivery file");

        group.MapDelete("deliveries/{deliveryId:guid}", async (IDeliveryService deliveryService, Guid deliveryId) =>
        {
            var result = await deliveryService.DeleteById(deliveryId);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteDelivery")
        .RequireAuthorization("InDelivery")
        .WithSummary("Delete delivery by id");
    }
}
