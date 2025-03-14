using System.Collections.Concurrent;

namespace Container;

public interface IAnalyzerCancellationStore
{
    CancellationToken RegisterToken(Guid id, CancellationToken cancellationToken);
    Task Cancel(Guid id);
    void Remove(Guid id);
}

public class AnalyzerCancellationStore : IAnalyzerCancellationStore
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _tokens = new();

    public CancellationToken RegisterToken(Guid id, CancellationToken cancellationToken)
    {
        var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _tokens[id] = source;
        return source.Token;
    }

    public async Task Cancel(Guid id)
    {
        if (_tokens.TryRemove(id, out var source))
        {
            await source.CancelAsync();
        }
    }

    public void Remove(Guid id)
    {
        _tokens.TryRemove(id, out _);
    }
}
