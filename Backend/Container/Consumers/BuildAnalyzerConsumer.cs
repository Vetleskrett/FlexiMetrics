using Container.Contracts;
using Database;
using Database.Models;
using MassTransit;
using System.Threading.Channels;

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

        var channel = Channel.CreateUnbounded<AnalyzerLog>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        var createImageTask = _containerService.CreateImage(analyzer, channel.Writer, context.CancellationToken);

        await foreach(var log in channel.Reader.ReadAllAsync(context.CancellationToken))
        {
            _dbContext.AnalyzerLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }

        await createImageTask;

        analyzer.State = AnalyzerState.Standby;
        await _dbContext.SaveChangesAsync();
    }
}