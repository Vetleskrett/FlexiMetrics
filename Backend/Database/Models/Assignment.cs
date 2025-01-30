using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Assignment
{
    [Key]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime DueDate { get; set; }
    public required bool Published { get; set; }
    public required CollaborationType CollaborationType { get; set; }
    public required bool Mandatory { get; set; }
    public required GradingFormat GradingFormat { get; set; }
    public required string Description { get; set; }
    public required Guid CourseId { get; set; }
    public Course? Course { get; set; }
    public List<AssignmentField>? Fields { get; set; }
}

[Owned]
public class GradingFormat
{
    public required GradingType GradingType { get; set; }
    public required int? MaxPoints { get; set; }
}

public enum GradingType
{
    NoGrading,
    ApprovalGrading,
    LetterGrading,
    PointsGrading
}