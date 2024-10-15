namespace Api.Assignments.Contracts
{
    public class AssignmentResponse
    {
            public required Guid Id { get; set; }
            public required string Name { get; set; }
            public required Guid CourseId { get; set; }
            public DateTime? DueDate { get; set; }
            public IEnumerable<AssignmentVariableContract>? Variables { get; set; }
    }
}
