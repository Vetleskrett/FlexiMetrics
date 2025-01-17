using Database.Models;

namespace Api.Assignments.Contracts;

public class UpdateAssignmentRequest
{
    public required string Name { get; set; }
    public required DateTime DueDate { get; set; }
    public required bool Published { get; set; }
    public required CollabrotationType CollabrotationType { get; set; }
}
