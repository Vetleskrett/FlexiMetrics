namespace Api.Teams.Contracts;

public class UpdateTeamRequest
{
    public required Guid CourseId { get; init; }
    public required int TeamId { get; init; }
}
