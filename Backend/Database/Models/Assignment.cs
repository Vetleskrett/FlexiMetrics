using System.ComponentModel.DataAnnotations;

namespace Database.Models;

public class Assignment : IModel
{
    [Key]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime DueDate { get; set; }
    public required bool Published { get; set; }
    public required CollaborationType CollaborationType { get; set; }
    public required Guid CourseId { get; set; }
    public Course? Course { get; set; }
    public List<AssignmentField>? Fields { get; set; }
}