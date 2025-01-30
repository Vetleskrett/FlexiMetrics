using Database.Models;

namespace Api.Feedbacks.Contracts;

public abstract class GradingRequest
{
}

public abstract class ApprovalGradingRequest : GradingRequest
{
    public required bool IsApproved { get; init; }
}

public abstract class LetterGradingRequest : GradingRequest
{
    public required LetterGrade LetterGrade { get; init; }
}

public abstract class PointsGradingRequest : GradingRequest
{
    public required int Points { get; init; }
}