using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class AssignmentField
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
    public required AssignmentDataType Type { get; set; }
    public required string Name { get; set; }

    public required int? Min { get; set; }
    public required int? Max { get; set; }
    public required string? Regex { get; set; }
    public required AssignmentDataType? SubType { get; set; }
}