using Api.AssignmentFields.Contracts;
using Database.Models;

namespace Api.Assignments.Contracts;

public class CreateAssignmentRequest
{
    public required string Name { get; set; }
    public required Guid CourseId { get; set; }
    public required DateTime DueDate { get; set; }
    public required bool Published { get; set; }
    public required CollabrotationType CollabrotationType { get; set; }
    public required List<CreateAssignmentFieldRequest> Fields { get; set; }
}
