﻿using Api.Assignments.Contracts;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Assignments;

public interface IAssignmentService
{
    Task<IEnumerable<AssignmentResponse>> GetAll();
    Task<IEnumerable<AssignmentResponse>> GetAllByCourse(Guid courseId);
    Task<IEnumerable<StudentAssignmentResponse>?> GetAllByStudentCourse(Guid studentId, Guid courseId);
    Task<AssignmentResponse?> GetById(Guid id);
    Task<Result<AssignmentResponse, ValidationResponse>> Create(CreateAssignmentRequest request);
    Task<Result<AssignmentResponse?, ValidationResponse>> Update(UpdateAssignmentRequest request, Guid id);
    Task<bool> DeleteById(Guid id);
}

public class AssignmentService : IAssignmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Assignment> _validator;

    public AssignmentService(AppDbContext dbContext, IValidator<Assignment> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<IEnumerable<AssignmentResponse>> GetAll()
    {
        var assignments = await _dbContext.Assignments
            .AsNoTracking()
            .ToListAsync();
        return assignments.MapToResponse();
    }

    public async Task<IEnumerable<AssignmentResponse>> GetAllByCourse(Guid courseId)
    {
        var assignments = await _dbContext.Assignments
            .Where(x => x.CourseId == courseId)
            .ToListAsync();
        return assignments.MapToResponse();
    }

    public async Task<IEnumerable<StudentAssignmentResponse>?> GetAllByStudentCourse(Guid studentId, Guid courseId)
    {
        var assignments = await _dbContext.Assignments
            .Where(x => x.CourseId == courseId && x.Published == true)
            .Select(a => new StudentAssignmentResponse
            {
                Id = a.Id,
                Name = a.Name,
                DueDate = a.DueDate,
                CollaborationType = a.CollaborationType,
                CourseId = a.CourseId,
                IsDelivered = _dbContext.Deliveries
                        .Where(d => d.AssignmentId == a.Id)
                        .Any(d =>
                            (d.StudentId == studentId) ||
                            (d.Team != null && d.Team.Students.Any(s => s.Id == studentId))
                        ),
            })
            .ToListAsync();

        return assignments;
    }

    public async Task<AssignmentResponse?> GetById(Guid id)
    {
        var assignment = await _dbContext.Assignments.FindAsync(id);
        return assignment?.MapToResponse();
    }

    public async Task<Result<AssignmentResponse, ValidationResponse>> Create(CreateAssignmentRequest request)
    {
        var assignment = request.MapToAssignment();
        assignment.Course = await _dbContext.Courses.FindAsync(assignment.CourseId);

        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.AssignmentFields.AddRange(assignment.Fields!);
        _dbContext.Assignments.Add(assignment);

        await _dbContext.SaveChangesAsync();

        return assignment.MapToResponse();
    }

    public async Task<Result<AssignmentResponse?, ValidationResponse>> Update(UpdateAssignmentRequest request, Guid id)
    {
        var assignment = await _dbContext.Assignments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (assignment is null)
        {
            return default;
        }

        assignment = request.MapToAssignment(assignment.Id, assignment.CourseId);

        var validationResult = await _validator.ValidateAsync(assignment);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Assignments.Update(assignment);
        await _dbContext.SaveChangesAsync();

        return assignment.MapToResponse();
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Assignments.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
