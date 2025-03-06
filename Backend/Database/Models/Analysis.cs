using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Analysis
{
    [Key]
    public required Guid Id { get; set; }
    public required DateTime StartedAt { get; set; }
    public required DateTime? CompletedAt { get; set; }
    public required AnalysisStatus Status { get; set; }
    public required Guid AnalyzerId { get; set; }
    public Analyzer? Analyzer { get; set; }
    public required List<DeliveryAnalysis>? DeliveryAnalyses { get; set; }
}