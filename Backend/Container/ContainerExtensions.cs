using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Container;

public static class ContainerExtensions
{
    public static void AddContainer(this IServiceCollection services)
    {
        services.AddSingleton<IDockerClient>(new DockerClientConfiguration().CreateClient());
        services.AddSingleton<IContainerService, ContainerService>();
        services.AddSingleton<IAnalyzerCancellationStore, AnalyzerCancellationStore>();
        services.AddScoped<IAnalyzerExecutor, AnalyzerExecutor>();
    }

    public static async Task InitializeContainer(this IHost app)
    {
        await app.Services.GetRequiredService<IContainerService>().Initialize();
    }
}
