using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Feedback
{
    [Key]
    public required Guid Id { get; set; }
    public required string Comment { get; set; }
    public required Guid DeliveryId { get; set; }
    public Delivery? Delivery { get; set; }
}

public class ApprovalFeedback : Feedback
{
    public required bool IsApproved { get; set; }
}

public enum LetterGrade
{
    A, B, C, D, E, F
}

public class LetterFeedback : Feedback
{
    public required LetterGrade LetterGrade { get; set; }
}

public class PointsFeedback : Feedback
{
    public required int Points { get; set; }
}