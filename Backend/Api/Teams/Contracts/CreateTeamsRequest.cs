namespace Api.Teams.Contracts;

public class CreateTeamsRequest
{
    public required Guid CourseId { get; init; }
    public required int NumTeams { get; init; }
}
