using Container.Contracts;
using Database;
using Database.Models;
using FileStorage;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Container.Consumers;

public class CancelAnalyzerConsumer : IConsumer<CancelAnalyzerRequest>
{
    private readonly IAnalyzerCancellationStore _analyzerCancellationStore;
    private readonly AppDbContext _dbContext;
    private readonly IBus _bus;
    private readonly IFileStorage _fileStorage;

    public CancelAnalyzerConsumer(IAnalyzerCancellationStore analyzerCancellationStore, AppDbContext dbContext, IBus bus, IFileStorage fileStorage)
    {
        _analyzerCancellationStore = analyzerCancellationStore;
        _dbContext = dbContext;
        _bus = bus;
        _fileStorage = fileStorage;
    }

    public async Task Consume(ConsumeContext<CancelAnalyzerRequest> context)
    {
        var request = context.Message;

        await _analyzerCancellationStore.Cancel(request.AnalyzerId);

        var analysis = await _dbContext.Analyses
            .Include(a => a.Analyzer!)
            .ThenInclude(a => a.Assignment)
            .FirstOrDefaultAsync(a => a.Status == AnalysisStatus.Canceled && a.AnalyzerId == request.AnalyzerId);

        if (analysis is not null)
        {
            _fileStorage.DeleteAnalysis(analysis.Analyzer!.Assignment!.CourseId, analysis.Analyzer.AssignmentId, analysis.AnalyzerId, analysis.Id);

            await _dbContext.Analyses.Where(a => a.Id == analysis.Id).ExecuteDeleteAsync();
        }
    }
}