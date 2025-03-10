﻿using Container.Models;
using Docker.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;

namespace Container;

public static class ContainerExtensions
{
    public static void AddContainer(this IServiceCollection services)
    {
        services.AddSingleton<IDockerClient>(new DockerClientConfiguration().CreateClient());
        services.AddSingleton<IContainerService, ContainerService>();
        services.AddScoped<IAnalyzerExecutor, AnalyzerExecutor>();

        services.AddSingleton(Channel.CreateUnbounded<RunAnalyzerRequest>(new()
        {
            SingleReader = true,
            SingleWriter = false
        }));
        services.AddHostedService<RunAnalyzerWorker>();

        services.AddSingleton(Channel.CreateUnbounded<AnalysisStatusUpdate>(new()
        {
            SingleReader = false,
            SingleWriter = false
        }));
    }
}
