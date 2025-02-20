using Api.Deliveries.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Deliveries;

public static class DeliveryEndpoints
{
    public static void MapDeliveryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Deliveries");

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
        .WithSummary("Get delivery by id");

        group.MapGet("students/{studentId:guid}/assignments/{assignmentId:guid}/deliveries",
            async (IDeliveryService deliveryService, Guid studentId, Guid assignmentId) =>
        {
            var result = await deliveryService.GetByStudentAssignment(studentId, assignmentId);
            return result.MapToResponse(delivery => Results.Ok(delivery));
        })
        .Produces<DeliveryResponse>()
        .WithName("GetDeliveryByStudentAssignment")
        .WithSummary("Get delivery by student id and assignment id");

        group.MapGet("teams/{teamId:guid}/assignments/{assignmentId:guid}/deliveries",
            async (IDeliveryService deliveryService, Guid teamId, Guid assignmentId) =>
        {
            var result = await deliveryService.GetByTeamAssignment(teamId, assignmentId);
            return result.MapToResponse(delivery => Results.Ok(delivery));
        })
        .Produces<DeliveryResponse>()
        .WithName("GetDeliveryByTeamAssignment")
        .WithSummary("Get delivery by team id and assignment id");

        group.MapGet("assignments/{assignmentId:guid}/deliveries", async (IDeliveryService deliveryService, Guid assignmentId) =>
        {
            var result = await deliveryService.GetAllByAssignment(assignmentId);
            return result.MapToResponse(deliveries => Results.Ok(deliveries));
        })
        .Produces<IEnumerable<DeliveryResponse>>()
        .WithName("GetAllDeliveriesByAssignment")
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

        group.MapPut("deliveries/{id}", async (IDeliveryService deliveryService, Guid id, UpdateDeliveryRequest request) =>
        {
            var result = await deliveryService.Update(request, id);
            return result.MapToResponse(delivery => Results.Ok(delivery));
        })
        .Produces<DeliveryResponse>()
        .WithName("UpdateDelivery")
        .WithSummary("Update delivery by id");

        group.MapGet("delivery-fields/{deliveryFieldId:guid}", async (IDeliveryService deliveryService, Guid deliveryFieldId) =>
        {
            var result = await deliveryService.DownloadFile(deliveryFieldId);
            return result.MapToResponse(file => Results.File(file.Stream, file.Metadata.ContentType, file.Metadata.FileName));
        })
        .Produces<FileStreamHttpResult>()
        .WithName("DownloadDeliveryFile")
        .WithSummary("Download delivery file");

        group.MapPost("delivery-fields/{deliveryFieldId:guid}", async (IDeliveryService deliveryService, IFormFile file, Guid deliveryFieldId) =>
        {
            var result = await deliveryService.UploadFile(file, deliveryFieldId);
            return result.MapToResponse(() => Results.Ok());
        })
        .Accepts<IFormFile>("multipart/form-data")
        .DisableAntiforgery()
        .WithName("UploadDeliveryFile")
        .WithSummary("Upload delivery file");

        group.MapDelete("deliveries/{id:guid}", async (IDeliveryService deliveryService, Guid id) =>
        {
            var result = await deliveryService.DeleteById(id);
            return result.MapToResponse(() => Results.Ok());
        })
        .WithName("DeleteDelivery")
        .WithSummary("Delete delivery by id");
    }
}
