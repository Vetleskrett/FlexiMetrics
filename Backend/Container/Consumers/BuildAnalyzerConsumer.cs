using Container.Contracts;
using Database;
using MassTransit;

namespace Container.Consumers;

public class BuildAnalyzerConsumer : IConsumer<BuildAnalyzerRequest>
{
    private readonly AppDbContext _dbContext;
    private readonly IContainerService _containerService;
    private readonly IBus _bus;

    public BuildAnalyzerConsumer(AppDbContext dbContext, IContainerService containerService, IBus bus)
    {
        _dbContext = dbContext;
        _containerService = containerService;
        _bus = bus;
    }

    public async Task Consume(ConsumeContext<BuildAnalyzerRequest> context)
    {
        var request = context.Message;
        var analyzer = await _dbContext.Analyzers.FindAsync(request.AnalyzerId);

        if (analyzer is null)
        {
            return;
        }

        await _containerService.CreateImage(analyzer, context.CancellationToken);

        analyzer.State = Database.Models.AnalyzerState.Standby;
        await _dbContext.SaveChangesAsync();

        await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId));
    }
}