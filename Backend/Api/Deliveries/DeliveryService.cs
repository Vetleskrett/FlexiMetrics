using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;
using Api.Deliveries.Contracts;
using FileStorage;
using Api.Common;

namespace Api.Deliveries;

public interface IDeliveryService
{
    Task<Result<IEnumerable<DeliveryResponse>>> GetAll();
    Task<Result<DeliveryResponse>> GetById(Guid id);
    Task<Result<DeliveryResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId);
    Task<Result<DeliveryResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId);
    Task<Result<IEnumerable<DeliveryResponse>>> GetAllByAssignment(Guid assignmentId);
    Task<Result<DeliveryResponse>> Create(CreateDeliveryRequest request);
    Task<Result<DeliveryResponse>> Update(UpdateDeliveryRequest request, Guid id);
    Task<Result> UploadFile(IFormFile file, Guid deliveryFieldId);
    Task<Result<FileResponse>> DownloadFile(Guid deliveryFieldId);
    Task<Result> DeleteById(Guid id);
}

public class DeliveryService : IDeliveryService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Delivery> _validator;
    private readonly IFileStorage _fileStorage;

    public DeliveryService(AppDbContext dbContext, IValidator<Delivery> validator, IFileStorage fileStorage)
    {
        _dbContext = dbContext;
        _validator = validator;
        _fileStorage = fileStorage;
    }

    public async Task<Result<IEnumerable<DeliveryResponse>>> GetAll()
    {
        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .OrderBy(d => d.Student!.Email)
            .ThenBy(d => d.Team!.TeamNr)
            .AsNoTracking()
            .ToListAsync();

        return deliveries.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> GetById(Guid id)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        return delivery.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> GetByStudentAssignment(Guid studentId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(studentId);
        if (student is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.CourseId == assignment.CourseId && cs.StudentId == studentId);
        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        var query = _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .Where(d => d.AssignmentId == assignmentId);

        Delivery? delivery;
        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            delivery = await query
                .FirstOrDefaultAsync(d => d.StudentId == studentId);
        }
        else
        {
            delivery = await query
                .FirstOrDefaultAsync(d => d.Team!.Students.Any(s => s.Id == studentId));
        }

        if (delivery is null)
        {
            return Result<DeliveryResponse>.NoContent();
        }
        return delivery.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> GetByTeamAssignment(Guid teamId, Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        if (team.CourseId != assignment.CourseId)
        {
            return new ValidationError("Team is not in the course").MapToResponse();
        }

        if (assignment.CollaborationType == CollaborationType.Individual)
        {
            return new ValidationError("Assignment is individual").MapToResponse();
        }

        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .FirstOrDefaultAsync(d => d.AssignmentId == assignmentId && d.TeamId == teamId);

        if (delivery is null)
        {
            return Result<DeliveryResponse>.NoContent();
        }
        return delivery.MapToResponse();
    }

    public async Task<Result<IEnumerable<DeliveryResponse>>> GetAllByAssignment(Guid assignmentId)
    {
        var assignment = await _dbContext.Assignments.FindAsync(assignmentId);
        if (assignment is null)
        {
            return Result<IEnumerable<DeliveryResponse>>.NotFound();
        }

        var deliveries = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentFieldId))
            .OrderBy(d => d.Student!.Email)
            .ThenBy(d => d.Team!.TeamNr)
            .Where(d => d.AssignmentId == assignmentId)
            .ToListAsync();

        return deliveries.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> Create(CreateDeliveryRequest request)
    {
        var assignment = await _dbContext.Assignments
            .Include(a => a.Fields)
            .FirstOrDefaultAsync(a => a.Id == request.AssignmentId);
        if (assignment is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var student = await _dbContext.Users.FindAsync(request.StudentId);
        if (student is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        var courseStudent = await _dbContext.CourseStudents
            .FirstOrDefaultAsync(cs => cs.CourseId == assignment.CourseId && cs.StudentId == student.Id);
        if (courseStudent is null)
        {
            return new ValidationError("Student is not enrolled in the course").MapToResponse();
        }

        if (assignment.DueDate < DateTime.UtcNow)
        {
            return new ValidationError("Cannot deliver after assignment due date").MapToResponse();
        }

        Delivery delivery;

        if (assignment.CollaborationType == CollaborationType.Teams)
        {
            var team = await _dbContext.Teams
                .Where(t => t.CourseId == assignment.CourseId)
                .Where(t => t.Students.Any(s => s.Id == request.StudentId))
                .FirstOrDefaultAsync();

            if (team is null)
            {
                return Result<DeliveryResponse>.NotFound();
            }

            var existingDelivery = await _dbContext.Deliveries
                .AnyAsync(d => d.AssignmentId == assignment.Id && d.TeamId == team.Id);
            if (existingDelivery)
            {
                return new ValidationError("Delivery already exists").MapToResponse();
            }

            delivery = request.MapToTeamDelivery(team.Id);
        }
        else
        {
            var existingDelivery = await _dbContext.Deliveries
                .AnyAsync(d => d.AssignmentId == assignment.Id && d.StudentId == student.Id);
            if (existingDelivery)
            {
                return new ValidationError("Delivery already exists").MapToResponse();
            }

            delivery = request.MapToStudentDelivery();
        }

        delivery.Assignment = assignment;

        var validationResult = await _validator.ValidateAsync(delivery);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Deliveries.Add(delivery);
        await _dbContext.SaveChangesAsync();

        return delivery.MapToResponse();
    }

    public async Task<Result<DeliveryResponse>> Update(UpdateDeliveryRequest request, Guid id)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Fields!.OrderBy(f => f.AssignmentField!.Type))
            .Include(d => d.Assignment!)
            .ThenInclude(a => a.Fields)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery is null)
        {
            return Result<DeliveryResponse>.NotFound();
        }

        if (delivery.Assignment!.DueDate < DateTime.UtcNow)
        {
            return new ValidationError("Cannot deliver after assignment due date").MapToResponse();
        }

        foreach (var fieldRequest in request.Fields)
        {
            var assignmentField = delivery.Assignment!.Fields!.FirstOrDefault(f => f.Id == fieldRequest.AssignmentFieldId);
            if (assignmentField is null)
            {
                return Result<DeliveryResponse>.NotFound();
            }

            var deliveryField = delivery.Fields!.FirstOrDefault(f => f.AssignmentFieldId == fieldRequest.AssignmentFieldId);
            if (deliveryField is null)
            {
                deliveryField = fieldRequest.MapToDeliveryField(delivery.Id);
                delivery.Fields!.Add(deliveryField);
            }
            else
            {
                deliveryField.Value = fieldRequest.Value;
            }
        }

        var validationResult = await _validator.ValidateAsync(delivery);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        await _dbContext.SaveChangesAsync();

        return delivery.MapToResponse();
    }

    public async Task<Result> UploadFile(IFormFile file, Guid deliveryFieldId)
    {
        var deliveryField = await _dbContext.DeliveryFields
            .Include(f => f.AssignmentField)
            .Include(f => f.Delivery!)
            .ThenInclude(d => d.Assignment)
            .FirstOrDefaultAsync(f => f.Id == deliveryFieldId);

        if (deliveryField is null)
        {
            return Result.NotFound();
        }

        if (deliveryField.AssignmentField!.Type != AssignmentDataType.File)
        {
            return new ValidationError("Assignment field type is not file").MapToResponse();
        }

        try
        {
            await _fileStorage.WriteDeliveryField
            (
                deliveryField.Delivery!.Assignment!.CourseId,
                deliveryField.Delivery!.AssignmentId,
                deliveryField.DeliveryId,
                deliveryFieldId,
                file.OpenReadStream()
            );
        }
        catch (Exception e)
        {
            return new ValidationError($"Could not save file: {e.Message}").MapToResponse();
        }

        return Result.Success();
    }

    public async Task<Result<FileResponse>> DownloadFile(Guid deliveryFieldId)
    {
        var deliveryField = await _dbContext.DeliveryFields
            .Include(f => f.AssignmentField)
            .Include(f => f.Delivery!)
            .ThenInclude(d => d.Assignment)
            .FirstOrDefaultAsync(f => f.Id == deliveryFieldId);

        if (deliveryField is null)
        {
            return Result<FileResponse>.NotFound();
        }

        try
        {
            var metadata = deliveryField.GetValue<FileMetadata>();

            var stream = _fileStorage.GetDeliveryField
            (
                deliveryField.Delivery!.Assignment!.CourseId,
                deliveryField.Delivery!.AssignmentId,
                deliveryField.DeliveryId,
                deliveryFieldId
            );

            return new FileResponse
            {
                Stream = stream,
                Metadata = metadata!
            };
        }
        catch (Exception e)
        {
            return new ValidationError($"Could not read file: {e.Message}").MapToResponse();
        }
    }

    public async Task<Result> DeleteById(Guid id)
    {
        var delivery = await _dbContext.Deliveries
            .Include(d => d.Assignment)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (delivery is null)
        {
            return Result.NotFound();
        }

        _fileStorage.DeleteDelivery(delivery.Assignment!.CourseId, delivery.AssignmentId, delivery.Id);

        await _dbContext.Deliveries.Where(x => x.Id == id).ExecuteDeleteAsync();
        return Result.Success();
    }
}
