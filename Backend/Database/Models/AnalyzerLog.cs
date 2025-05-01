using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class AnalyzerLog
{
    [Key]
    public required Guid Id { get; set; }
    public required DateTime Timestamp { get; set; }
    public required AnalyzerLogType Type { get; set; }
    public required string Category { get; set; }
    public required string Text { get; set; }
    public required Guid AnalyzerId { get; set; }
    public Analyzer? Analyzer { get; set; }
}