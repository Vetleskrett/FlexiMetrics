using Container.Models;
using MassTransit;
using System.Threading.Channels;

namespace Api.Analyzers;

public interface IAnalyzerStatusUpdateReader
{
    Channel<AnalyzerStatusUpdate> GetChannel();
    void RemoveChannel(Channel<AnalyzerStatusUpdate> channel);
    Task Consume(AnalyzerStatusUpdate statusUpdate);
}

public class AnalyzerStatusUpdateReader : IAnalyzerStatusUpdateReader
{
    private readonly List<Channel<AnalyzerStatusUpdate>> _channels = [];

    public Channel<AnalyzerStatusUpdate> GetChannel()
    {
        var channel = Channel.CreateUnbounded<AnalyzerStatusUpdate>();
        _channels.Add(channel);
        return channel;
    }

    public void RemoveChannel(Channel<AnalyzerStatusUpdate> channel)
    {
        _channels.Remove(channel);
    }

    public async Task Consume(AnalyzerStatusUpdate statusUpdate)
    {
        foreach (var channel in _channels.ToArray())
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

