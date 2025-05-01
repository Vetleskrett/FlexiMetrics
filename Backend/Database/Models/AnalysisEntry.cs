using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class AnalysisEntry
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid AnalysisId { get; set; }
    public Analysis? Analysis { get; set; }
    public required Guid? StudentId { get; set; }
    public User? Student { get; set; }
    public required Guid? TeamId { get; set; }
    public Team? Team { get; set; }
    public required List<AnalysisField> Fields { get; set; }
    public required DateTime? CompletedAt { get; set; }
}