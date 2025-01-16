namespace Api.Teams.Contracts;

public class BulkAddStudentToTeamsRequest
{
    public required Guid CourseId { get; init; }
    public required List<TeamRequest> Teams { get; init; }
}

public class TeamRequest
{
    public required int TeamNr { get; init; }
    public required List<string> Emails { get; init; }
}