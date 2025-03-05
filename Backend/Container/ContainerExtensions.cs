using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace Container;

public static class ContainerExtensions
{
    public static void AddContainer(this IServiceCollection services)
    {
        services.AddSingleton<IDockerClient>(new DockerClientConfiguration().CreateClient());
        services.AddScoped<IContainerService, ContainerService>();
        services.AddSingleton(Channel.CreateUnbounded<RunAnalyzerRequest>(new()
        {
            SingleReader = true,
            SingleWriter = false
        }));
        services.AddHostedService<ContainerWorker>();
    }
}
