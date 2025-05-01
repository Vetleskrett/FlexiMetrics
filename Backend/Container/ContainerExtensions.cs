using Container.Consumers;
using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;

namespace Container;

public static class ContainerExtensions
{
    public static void AddContainer(this IServiceCollection services)
    {
        services.AddSingleton<IDockerClient>(new DockerClientConfiguration().CreateClient());
        services.AddSingleton<IContainerService, ContainerService>();
        services.AddSingleton<IAnalyzerCancellationStore, AnalyzerCancellationStore>();
        services.AddScoped<IAnalyzerExecutor, AnalyzerExecutor>();
        services.AddHostedService<StartupBuildWorker>();
    }
}