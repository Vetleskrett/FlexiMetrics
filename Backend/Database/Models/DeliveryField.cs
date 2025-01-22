using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class DeliveryField
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid DeliveryId { get; set; }
    public Delivery? Delivery { get; set; }
    public required Guid AssignmentFieldId { get; set; }
    public AssignmentField? AssignmentField { get; set; }
    public required string Value { get; set; }
}
