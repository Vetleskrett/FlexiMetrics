using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Analyzer
{
    [Key]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string FileName { get; set; }
    public required Guid AssignmentId { get; set; }
    public Assignment? Assignment { get; set; }
}
