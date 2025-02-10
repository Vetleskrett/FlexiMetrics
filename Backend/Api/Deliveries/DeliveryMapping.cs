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
            StudentId = request.StudentId,
            TeamId = null,
            Fields = request.Fields.MapToDeliveryFields(id),
        };
    }

    public static Delivery MapToTeamDelivery(this CreateDeliveryRequest request, Guid teamId)
    {
        var id = Guid.NewGuid();
        return new Delivery
        {
            Id = id,
            AssignmentId = request.AssignmentId,
            TeamId = teamId,
            StudentId = null,
            Fields = request.Fields.MapToDeliveryFields(id),
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

    public static List<DeliveryField> MapToDeliveryFields(this IEnumerable<CreateDeliveryFieldRequest> fields, Guid deliveryId)
    {
        return fields.Select(field => field.MapToDeliveryField(deliveryId)).ToList();
    }

    public static DeliveryResponse MapToResponse(this Delivery delivery)
    {
        return new DeliveryResponse
        {
            Id = delivery.Id,
            AssignmentId = delivery.AssignmentId,
            StudentId = delivery.StudentId,
            TeamId = delivery.TeamId,
            Fields = delivery.Fields!.MapToResponse(),
        };
    }

    public static List<DeliveryResponse> MapToResponse(this IEnumerable<Delivery> deliveries)
    {
        return deliveries.Select(delivery => delivery.MapToResponse()).ToList();
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

    public static List<DeliveryFieldResponse> MapToResponse(this IEnumerable<DeliveryField> fields)
    {
        return fields.Select(field => field.MapToResponse()).ToList();
    }
}
