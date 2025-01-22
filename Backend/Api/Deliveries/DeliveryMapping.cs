using Api.Deliveries.Contracts;
using Database.Models;

namespace Api.Deliveries;

public static class DeliveryMapping
{
    public static StudentDelivery MapToStudentDelivery(this CreateDeliveryRequest request)
    {
        var id = Guid.NewGuid();
        return new StudentDelivery
        {
            Id = id,
            AssignmentId = request.AssignmentId,
            StudentId = request.StudentOrTeamId,
            Fields = request.Fields.MapToDeliveryFields(id).ToList(),
        };
    }

    public static TeamDelivery MapToTeamDelivery(this CreateDeliveryRequest request)
    {
        var id = Guid.NewGuid();
        return new TeamDelivery
        {
            Id = id,
            AssignmentId = request.AssignmentId,
            TeamId = request.StudentOrTeamId,
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
        if (delivery is StudentDelivery studentDelivery)
        {
            return studentDelivery.MapToResponse();
        }

        if (delivery is TeamDelivery teamDelivery)
        {
            return teamDelivery.MapToResponse();
        }

        return new DeliveryResponse
        {
            Id = delivery.Id,
            AssignmentId = delivery.AssignmentId,
            StudentId = null,
            TeamId = null,
            Fields = delivery.Fields!.MapToResponse().ToList(),
        };
    }

    public static DeliveryResponse MapToResponse(this StudentDelivery delivery)
    {
        return new DeliveryResponse
        {
            Id = delivery.Id,
            AssignmentId = delivery.AssignmentId,
            StudentId = delivery.StudentId,
            TeamId = null,
            Fields = delivery.Fields!.MapToResponse().ToList(),
        };
    }

    public static DeliveryResponse MapToResponse(this TeamDelivery delivery)
    {
        return new DeliveryResponse
        {
            Id = delivery.Id,
            AssignmentId = delivery.AssignmentId,
            StudentId = null,
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
