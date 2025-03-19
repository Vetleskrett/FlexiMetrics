using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Database.Models;

public class AnalysisField
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid AnalysisEntryId { get; set; }
    public AnalysisEntry? AnalysisEntry { get; set; }
    public required string Name { get; set; }
    public required AnalysisFieldType Type { get; set; }
    public required AnalysisFieldType? SubType { get; set; }
    public JsonDocument? JsonValue { get; set; }

    [NotMapped]
    public required object Value
    {
        get => JsonValue!.Deserialize<object>()!;
        set => JsonValue = JsonSerializer.SerializeToDocument(value);
    }

    public T GetValue<T>()
    {
        return JsonValue!.Deserialize<T>()!;
    }
}