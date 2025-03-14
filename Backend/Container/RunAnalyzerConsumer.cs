using Container.Models;
using MassTransit;

namespace Container;

public class RunAnalyzerConsumer : IConsumer<RunAnalyzerRequest>
{
    private readonly IAnalyzerExecutor _analyzerExecutor;
    private readonly IAnalyzerCancellationStore _analyzerCancellationStore;

    public RunAnalyzerConsumer(IAnalyzerExecutor analyzerExecutor, IAnalyzerCancellationStore analyzerCancellationStore)
    {
        _analyzerExecutor = analyzerExecutor;
        _analyzerCancellationStore = analyzerCancellationStore;
    }

    public async Task Consume(ConsumeContext<RunAnalyzerRequest> context)
    {
        var request = context.Message;
        var token = _analyzerCancellationStore.RegisterToken(request.AnalyzerId, context.CancellationToken);
        try
        {
            await _analyzerExecutor.RunAnalyzer(request, token);
        }
        finally
        {
            _analyzerCancellationStore.Remove(request.AnalyzerId);
        }
    }
}
