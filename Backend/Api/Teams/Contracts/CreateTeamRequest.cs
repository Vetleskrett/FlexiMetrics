namespace Api.Teams.Contracts;

public class CreateTeamRequest
{
    public required Guid CourseId { get; init; }
    public required int TeamNr { get; init; }
}
