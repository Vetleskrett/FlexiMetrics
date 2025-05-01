using Container.Contracts;
using Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Container;

public class StartupBuildWorker(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = scopeFactory.CreateScope();
        var bus = scope.ServiceProvider.GetRequiredService<IBus>();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var analyzerIds = await dbContext.Analyzers
            .Select(a => a.Id)
            .ToListAsync();

        await Task.WhenAll
        (
            analyzerIds.Select(id => bus.Publish(new BuildAnalyzerRequest(id)))
        );
    }
}