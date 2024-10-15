using Database.Models;

namespace Api.Assignments.Contracts
{
    public class AssignmentVariableContract
    {
        public required VariableType Type { get; set; }
        public required string Name { get; set; }
    }
}