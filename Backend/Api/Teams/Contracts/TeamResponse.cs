using Database.Models;

namespace Api.Teams.Contracts
{
    public class TeamResponse
    {
        public required Guid Id { get; init; }
        public required Guid CourseId { get; init; }
        public required int TeamId { get; init; }
    }
}
