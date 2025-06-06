﻿using Container.Contracts;
using MassTransit;

namespace Container.Consumers;

public class DeleteAnalyzerConsumer : IConsumer<DeleteAnalyzerRequest>
{
    private readonly IContainerService _containerService;

    public DeleteAnalyzerConsumer(IContainerService containerService)
    {
        _containerService = containerService;
    }

    public async Task Consume(ConsumeContext<DeleteAnalyzerRequest> context)
    {
        var request = context.Message;
        await _containerService.DeleteImage(request.AnalyzerId);
    }
}