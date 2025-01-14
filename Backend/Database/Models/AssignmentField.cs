using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class AssignmentField
{
    [Key]
    public required Guid Id { get; set; }
    public required AssignmentDataType Type { get; set; }
    public required string Name { get; set; }
    public Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
}
