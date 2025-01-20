using Database.Models;

namespace Api.Assignments.Contracts;

public class AssignmentResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime DueDate { get; set; }
    public required bool Published { get; set; }
    public required CollaborationType CollaborationType { get; set; }
    public required Guid CourseId { get; set; }
}
