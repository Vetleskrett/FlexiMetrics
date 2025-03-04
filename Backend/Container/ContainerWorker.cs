﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace Container;

class ContainerWorker : BackgroundService
{
    private readonly Channel<RunAnalyzerRequest> _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ContainerWorker> _logger;

    public ContainerWorker(Channel<RunAnalyzerRequest> channel, IServiceScopeFactory serviceScopeFactory, ILogger<ContainerWorker> logger)
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

        List<Task> tasks = [];
        while (await _channel.Reader.WaitToReadAsync(cancellationToken))
        {
            foreach (var task in tasks.Where(t => t.IsCompleted).ToList())
            {
                await task;
                tasks.Remove(task);
            }

            var request = await _channel.Reader.ReadAsync(cancellationToken);
            tasks.Add(Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var containerService = scope.ServiceProvider.GetRequiredService<IContainerService>();
                try
                {
                    await containerService.RunAnalyzer(request, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError("Error running analyzer: {ERROR}", e);
                }
            }, cancellationToken));
        }
    }
}
