using Database.Models;

namespace Container.Models;

public class OutputField
{
    public required AnalysisFieldType Type { get; init; }
    public AnalysisFieldType? SubType { get; init; }
    public required object Value { get; init; }
}