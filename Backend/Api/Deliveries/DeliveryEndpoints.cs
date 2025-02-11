using Api.Deliveries.Contracts;

namespace Api.Deliveries;

public static class DeliveryEndpoints
{
    public static void MapDeliveryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("").WithTags("Deliveries");

        group.MapGet("deliveries", async (IDeliveryService deliveryService) =>
        {
            var deliveries = await deliveryService.GetAll();
            return Results.Ok(deliveries);
        })
        .Produces<IEnumerable<DeliveryResponse>>()
        .WithName("GetAllDeliveries")
        .WithSummary("Get all deliveries");

        group.MapGet("deliveries/{id:guid}",
            async (IDeliveryService deliveryService, Guid id) =>
            {
                var delivery = await deliveryService.GetById(id);
                return delivery is not null ? Results.Ok(delivery) : Results.NotFound();
            })
        .Produces<DeliveryResponse>()
        .WithName("GetDelivery")
        .WithSummary("Get delivery by id");

        group.MapGet("students/{studentId:guid}/assignments/{assignmentId:guid}/deliveries",
            async (IDeliveryService deliveryService, Guid studentId, Guid assignmentId) =>
        {
            var result = await deliveryService.GetByStudentAssignment(studentId, assignmentId);
            return result.Match
            (
                delivery => delivery is not null ? Results.Ok(delivery) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<DeliveryResponse>()
        .WithName("GetDeliveryByStudentAssignment")
        .WithSummary("Get delivery by student id and assignment id");

        group.MapGet("teams/{teamId:guid}/assignments/{assignmentId:guid}/deliveries",
            async (IDeliveryService deliveryService, Guid teamId, Guid assignmentId) =>
        {
            var result = await deliveryService.GetByTeamAssignment(teamId, assignmentId);
            return result.Match
            (
                delivery => delivery is not null ? Results.Ok(delivery) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<DeliveryResponse>()
        .WithName("GetDeliveryByTeamAssignment")
        .WithSummary("Get delivery by team id and assignment id");

        group.MapGet("assignments/{assignmentId:guid}/deliveries", async (IDeliveryService deliveryService, Guid assignmentId) =>
        {
            var deliveries = await deliveryService.GetAllByAssignment(assignmentId);
            return deliveries is not null ? Results.Ok(deliveries) : Results.NotFound();
        })
        .Produces<IEnumerable<DeliveryResponse>>()
        .WithName("GetAllDeliveriesByAssignment")
        .WithSummary("Get all deliveries by assignment id");

        group.MapPost("deliveries", async (IDeliveryService deliveryService, CreateDeliveryRequest request) =>
        {
            var result = await deliveryService.Create(request);

            return result.Match
            (
                delivery => delivery is not null ? Results.CreatedAtRoute
                (
                    "GetDelivery",
                    new { id = delivery.Id },
                    delivery
                ) : Results.NotFound(),
                failure => Results.BadRequest(failure)
            );
        })
        .Produces<DeliveryResponse>()
        .WithName("CreateDelivery")
        .WithSummary("Create new delivery");

        group.MapDelete("deliveries/{id:guid}", async (IDeliveryService deliveryService, Guid id) =>
        {
            var deleted = await deliveryService.DeleteById(id);
            return deleted ? Results.Ok() : Results.NotFound();
        })
        .WithName("DeleteDelivery")
        .WithSummary("Delete delivery by id");
    }
}
