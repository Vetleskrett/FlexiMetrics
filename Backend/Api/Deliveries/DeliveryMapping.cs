using Api.Deliveries.Contracts;
using Database.Models;

namespace Api.Deliveries;

public static class DeliveryMapping
{
    public static Delivery MapToStudentDelivery(this CreateDeliveryRequest request)
    {
        var id = Guid.NewGuid();
        return new Delivery
        {
            Id = id,
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentOrTeamId,
            TeamId = null,
            Fields = request.Fields.MapToDeliveryFields(id).ToList(),
        };
    }

    public static Delivery MapToTeamDelivery(this CreateDeliveryRequest request)
    {
        var id = Guid.NewGuid();
        return new Delivery
        {
            Id = id,
            AssignmentId = request.AssignmentId,
            TeamId = request.StudentOrTeamId,
            StudentId = null,
            Fields = request.Fields.MapToDeliveryFields(id).ToList(),
        };
    }

    public static DeliveryField MapToDeliveryField(this CreateDeliveryFieldRequest request, Guid deliveryId)
    {
        return new DeliveryField
        {
            Id = Guid.NewGuid(),
            DeliveryId = deliveryId,
            AssignmentFieldId = request.AssignmentFieldId,
            Value = request.Value,
        };
    }

    public static IEnumerable<DeliveryField> MapToDeliveryFields(this IEnumerable<CreateDeliveryFieldRequest> fields, Guid deliveryId)
    {
        return fields.Select(field => field.MapToDeliveryField(deliveryId));
    }

    public static DeliveryResponse MapToResponse(this Delivery delivery)
    {
        return new DeliveryResponse
        {
            Id = delivery.Id,
            AssignmentId = delivery.AssignmentId,
            StudentId = delivery.StudentId,
            TeamId = delivery.TeamId,
            Fields = delivery.Fields!.MapToResponse().ToList(),
        };
    }

    public static IEnumerable<DeliveryResponse> MapToResponse(this IEnumerable<Delivery> deliveries)
    {
        return deliveries.Select(delivery => delivery.MapToResponse());
    }

    public static DeliveryFieldResponse MapToResponse(this DeliveryField field)
    {
        return new DeliveryFieldResponse
        {
            Id = field.Id,
            AssignmentFieldId = field.AssignmentFieldId,
            Value = field.Value,
        };
    }

    public static IEnumerable<DeliveryFieldResponse> MapToResponse(this IEnumerable<DeliveryField> fields)
    {
        return fields.Select(field => field.MapToResponse());
    }
}
