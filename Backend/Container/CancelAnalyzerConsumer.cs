using Container.Models;
using Database;
using Database.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Container;

public class CancelAnalyzerConsumer : IConsumer<CancelAnalyzerRequest>
{
    private readonly IAnalyzerCancellationStore _analyzerCancellationStore;
    private readonly AppDbContext _dbContext;
    private readonly IBus _bus;

    public CancelAnalyzerConsumer(IAnalyzerCancellationStore analyzerCancellationStore, AppDbContext dbContext, IBus bus)
    {
        _analyzerCancellationStore = analyzerCancellationStore;
        _dbContext = dbContext;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<CancelAnalyzerRequest> context)
    {
        var request = context.Message;

        await _analyzerCancellationStore.Cancel(request.AnalyzerId);

        await _dbContext.Analyses
            .Where(a => a.AnalyzerId == request.AnalyzerId)
            .Where(a => a.Status == AnalysisStatus.Canceled)
            .ExecuteDeleteAsync();

        await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId));
    }
}