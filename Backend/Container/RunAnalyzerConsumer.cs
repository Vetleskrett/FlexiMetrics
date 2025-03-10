using Container.Models;
using MassTransit;

namespace Container;

public class RunAnalyzerConsumer : IConsumer<RunAnalyzerRequest>
{
    private readonly IAnalyzerExecutor _analyzerExecutor;

    public RunAnalyzerConsumer(IAnalyzerExecutor analyzerExecutor)
    {
        _analyzerExecutor = analyzerExecutor;
    }

    public async Task Consume(ConsumeContext<RunAnalyzerRequest> context)
    {
        var request = context.Message;
        await _analyzerExecutor.RunAnalyzer(request, context.CancellationToken);
    }
}