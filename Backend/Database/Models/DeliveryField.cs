using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Database.Models;

public class DeliveryField
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid DeliveryId { get; set; }
    public Delivery? Delivery { get; set; }
    public required Guid AssignmentFieldId { get; set; }
    public AssignmentField? AssignmentField { get; set; }
    public JsonDocument? JsonValue { get; set; }

    [NotMapped]
    public required object Value
    {
        get => JsonSerializer.Deserialize<JsonElement>(JsonValue!).Deserialize<object>()!;
        set => JsonValue = JsonSerializer.SerializeToDocument(value);
    }
}