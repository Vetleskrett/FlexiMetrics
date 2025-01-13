namespace Api.Assignments.Contracts;

public class UpdateAssignmentRequest
{
    public required string Name { get; set; }
    public required Guid CourseId { get; set; }
    public DateTime? DueDate { get; set; }
    public List<AssignmentFieldRequest>? Fields { get; set; }
}
