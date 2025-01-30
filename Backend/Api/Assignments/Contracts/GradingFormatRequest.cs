using Database.Models;

namespace Api.Assignments.Contracts;

public class GradingFormatRequest
{
    public required GradingType GradingType { get; init; }
    public int? MaxPoints { get; init; }
}