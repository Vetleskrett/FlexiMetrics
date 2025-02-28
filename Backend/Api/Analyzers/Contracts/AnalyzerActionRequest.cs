namespace Api.Analyzers.Contracts;

public class AnalyzerActionRequest
{
    public required AnalyzerAction Action { get; init; }
}

public enum AnalyzerAction
{
    Run,
    Cancel
}