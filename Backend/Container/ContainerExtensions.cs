using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace Container;

public static class ContainerExtensions
{
    public static void AddContainer(this IServiceCollection services)
    {
        services.AddSingleton<IDockerClient>(new DockerClientConfiguration().CreateClient());
        services.AddScoped<IContainerService, ContainerService>();
    }

    public static async Task InitializeContainer(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var containerService = scope.ServiceProvider.GetRequiredService<IContainerService>();
        await containerService.Initialize();
    }
}
