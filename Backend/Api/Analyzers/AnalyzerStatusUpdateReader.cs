using Container.Models;
using MassTransit;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Api.Analyzers;

public interface IAnalyzerStatusUpdateReader
{
    IAsyncEnumerable<AnalyzerStatusUpdate> ReadAllAsync(Guid analyzerId, CancellationToken cancellationToken);
    Task Consume(AnalyzerStatusUpdate statusUpdate);
}

public class AnalyzerStatusUpdateReader : IAnalyzerStatusUpdateReader
{
    private readonly ConcurrentDictionary<Guid, Channel<AnalyzerStatusUpdate>> _channels = [];

    public async IAsyncEnumerable<AnalyzerStatusUpdate> ReadAllAsync(Guid analyzerId, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<AnalyzerStatusUpdate>();
        _channels[id] = channel;

        try
        {
            await foreach (var statusUpdate in channel.Reader.ReadAllAsync(cancellationToken))
            {
                if (statusUpdate.AnalyzerId == analyzerId)
                {
                    yield return statusUpdate;
                }
            }
        }
        finally
        {
            _channels.TryRemove(id, out _);
        }
    }

    public async Task Consume(AnalyzerStatusUpdate statusUpdate)
    {
        foreach (var channel in _channels.Values)
        {
            await channel.Writer.WriteAsync(statusUpdate);
        }
    }
}

public class AnalyzerStatusUpdateConsumer(IAnalyzerStatusUpdateReader reader) : IConsumer<AnalyzerStatusUpdate>
{
    public async Task Consume(ConsumeContext<AnalyzerStatusUpdate> context)
    {
        await reader.Consume(context.Message);
    }
}

