using Api.Analyzers.Contracts;
using Api.Validation;
using Container;
using Database;
using FileStorage;

namespace Api.Analyzers;

public interface IAnalyzerActionService
{
    Task<Result> StartAction(AnalyzerActionRequest request, Guid id);
}

public class AnalyzerActionService : IAnalyzerActionService
{
    private readonly AppDbContext _dbContext;
    private readonly IFileStorage _fileStorage;
    private readonly IContainerService _containerService;

    public AnalyzerActionService(AppDbContext dbContext, IFileStorage fileStorage, IContainerService containerService)
    {
        _dbContext = dbContext;
        _fileStorage = fileStorage;
        _containerService = containerService;
    }

    public async Task<Result> StartAction(AnalyzerActionRequest request, Guid id)
    {
        if (request.Action == AnalyzerAction.Run)
        {
            await _containerService.StartAnalyzer(id);
            return Result.Success();
        }
        else
        {
            throw new NotImplementedException();

        }
    }
}
