using Container.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Container;

class RunAnalyzerWorker : BackgroundService
{
    private readonly Channel<RunAnalyzerRequest> _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RunAnalyzerWorker> _logger;

    public RunAnalyzerWorker(Channel<RunAnalyzerRequest> channel, IServiceScopeFactory serviceScopeFactory, ILogger<RunAnalyzerWorker> logger)
    {
        _channel = channel;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var containerService = scope.ServiceProvider.GetRequiredService<IContainerService>();
            await containerService.Initialize();
        }

        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            var request = await _channel.Reader.ReadAsync(cancellationToken);
            using var scope = _serviceScopeFactory.CreateScope();
            var analyzerExecutor = scope.ServiceProvider.GetRequiredService<IAnalyzerExecutor>();
            try
            {
                await analyzerExecutor.RunAnalyzer(request, cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Error running analyzer: {ERROR}", e);
            }
        }
    }
}
