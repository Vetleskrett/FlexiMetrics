namespace Api.Analyzers.Contracts;

public class UpdateAnalyzerRequest
{
    public required string Name { get; init; }
    public required string FileName { get; set; }
}
