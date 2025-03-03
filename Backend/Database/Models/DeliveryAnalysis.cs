using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class DeliveryAnalysis
{
    [Key]
    public required Guid Id { get; set; }
    public required Guid AnalysisId { get; set; }
    public Analysis? Analysis { get; set; }
    public required Guid DeliveryId { get; set; }
    public Delivery? Delivery { get; set; }
    public required List<DeliveryAnalysisField> Fields { get; set; }
}