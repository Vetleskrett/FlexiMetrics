﻿using Container.Contracts;
using Container.Models;
using Database;
using Database.Models;
using FileStorage;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Container;

public interface IAnalyzerExecutor
{
    Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken);
}

public partial class AnalyzerExecutor : IAnalyzerExecutor
{
    private readonly AppDbContext _dbContext;
    private readonly IContainerService _containerService;
    private readonly IFileStorage _fileStorage;
    private readonly SemaphoreSlim _dbLock = new(1);
    private readonly IBus _bus;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
        WriteIndented = true
    };

    public AnalyzerExecutor(AppDbContext dbContext, IContainerService containerService, IFileStorage fileStorage, IBus bus)
    {
        _dbContext = dbContext;
        _containerService = containerService;
        _fileStorage = fileStorage;
        _bus = bus;
    }

    public async Task RunAnalyzer(RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var analysisEntries = await _dbContext.AnalysisEntries
                .Include(ae => ae.Student)
                .Include(ae => ae.Team)
                .ThenInclude(t => t!.Students)
                .Where(ae => ae.AnalysisId == request.AnalysisId)
                .ToListAsync(cancellationToken);

            var deliveries = await _dbContext.Deliveries
                .Include(d => d.Fields!)
                .ThenInclude(f => f.AssignmentField)
                .Where(d => d.AssignmentId == request.AssignmentId)
                .ToListAsync(cancellationToken);

            var script = await _fileStorage.GetAnalyzerScript(request.CourseId, request.AssignmentId, request.AnalyzerId);

            await Parallel.ForEachAsync(analysisEntries, cancellationToken, async (analysisEntry, cancellationToken) =>
            {
                var delivery = deliveries.FirstOrDefault(d => d.StudentId == analysisEntry.StudentId && d.TeamId == analysisEntry.TeamId);
                await RunAnalysisEntry(analysisEntry, delivery, request, script, cancellationToken);
            });

            await _dbContext.Analyses
                .Where(a => a.Id == request.AnalysisId)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(a => a.Status, AnalysisStatus.Completed)
                    .SetProperty(a => a.CompletedAt, DateTime.UtcNow)
                , cancellationToken);

            await _dbContext.Analyzers
                .Where(a => a.Id == request.AnalyzerId)
                .ExecuteUpdateAsync(x => x.SetProperty(a => a.State, AnalyzerState.Standby), cancellationToken);

            await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch
        {
            await _dbContext.Analyses
                .Where(a => a.Id == request.AnalysisId)
                .ExecuteUpdateAsync(setter => setter.SetProperty(a => a.Status, AnalysisStatus.Failed), cancellationToken);

            await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId), cancellationToken);

            throw;
        }
    }

    private async Task RunAnalysisEntry(AnalysisEntry analysisEntry, Delivery? delivery, RunAnalyzerRequest request, string script, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var container = await _containerService.CreateContainer(request.AnalyzerId, analysisEntry.Id, CancellationToken.None);

        cancellationToken.Register(async () =>
        {
            await _containerService.RemoveContainer(container);
        });

        try
        {
            await _containerService.CopyFileToContainer(container, script, "script.py", cancellationToken);

            var assignmentEntryDTO = AssignmentEntryDTO.Create(analysisEntry.Student, analysisEntry.Team, delivery);
            var entryJson = JsonSerializer.Serialize(assignmentEntryDTO, _jsonOptions);
            await _containerService.CopyFileToContainer(container, entryJson, "input.json", cancellationToken);

            if (delivery?.Fields is not null)
            {
                foreach (var fileField in delivery.Fields.Where(f => f.AssignmentField!.Type == AssignmentDataType.File))
                {
                    var fileMetadata = fileField.GetValue<FileMetadata>();
                    var fileStream = _fileStorage.GetDeliveryField(request.CourseId, request.AssignmentId, fileField.DeliveryId, fileField.Id);
                    await _containerService.CopyFileToContainer(container, fileStream, fileMetadata.FileName, cancellationToken);
                }
            }

            await _containerService.StartContainer(container, cancellationToken);

            await _containerService.WaitForContainerCompletion(container, cancellationToken);

            await OnAnalysisEntryFinished(container, analysisEntry, request, cancellationToken);

            await _bus.Publish(new AnalyzerStatusUpdate(request.AnalyzerId), cancellationToken);
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await _containerService.RemoveContainer(container);
            }
        }
    }

    private async Task OnAnalysisEntryFinished(string container, AnalysisEntry analysisEntry, RunAnalyzerRequest request, CancellationToken cancellationToken)
    {
        using var outputStream = await _containerService.CopyFileFromContainer(container, "output.json");

        if (outputStream is null)
        {
            return;
        }

        var outputFields = await JsonSerializer.DeserializeAsync<Dictionary<string, OutputField>>(outputStream, _jsonOptions, cancellationToken);

        var analysisFields = outputFields!.Select(pair =>
            new AnalysisField
            {
                Id = Guid.NewGuid(),
                AnalysisEntryId = analysisEntry.Id,
                Name = pair.Key,
                Type = pair.Value.Type,
                SubType = pair.Value.SubType,
                Value = pair.Value.Value,
            }
        ).ToList();

        foreach (var fileField in analysisFields.Where(f => f.Type == AnalysisFieldType.File))
        {
            cancellationToken.ThrowIfCancellationRequested();
            var fileMetadata = fileField.GetValue<FileMetadata>();
            using var fileStream = await _containerService.CopyFileFromContainer(container, fileMetadata.FileName);
            if (fileStream is not null)
            {
                await _fileStorage.WriteAnalysisField(request.CourseId, request.AssignmentId, request.AnalyzerId, request.AnalysisId, analysisEntry.Id, fileField.Id, fileStream);
            }
        }

        await _dbLock.WaitAsync(cancellationToken);
        try
        {
            _dbContext.AnalysisFields.AddRange(analysisFields);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _dbLock.Release();
        }
    }
}
