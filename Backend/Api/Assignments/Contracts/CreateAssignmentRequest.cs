namespace Api.Assignments.Contracts
{
    public class CreateAssignmentRequest
    {
        public required string Name { get; set; }
        public required Guid CourseId { get; set; }
        public DateTime? DueDate { get; set; }
        public List<AssignmentVariableContract> Variables { get; set; }
    }
}
