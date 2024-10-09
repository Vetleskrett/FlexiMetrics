using Api.Utils;
using Api.Validation;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Database.Models;
using Database;

namespace Api.Courses;

public interface ICourseService
{
    Task<IEnumerable<Course>> GetAll();
    Task<Course?> GetById(Guid id);
    Task<Result<Course, ValidationFailed>> Create(Course course);
    Task<Result<Course?, ValidationFailed>> Update(Course course);
    Task<bool> DeleteById(Guid id);
}

public class CourseService : ICourseService
{
    private readonly AppDbContext _dbContext;
    private readonly IValidator<Course> _validator;

    public CourseService(AppDbContext dbContext, IValidator<Course> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    public async Task<IEnumerable<Course>> GetAll()
    {
        return await _dbContext.Courses.AsNoTracking().ToListAsync();
    }

    public async Task<Course?> GetById(Guid id)
    {
        return await _dbContext.Courses.FindAsync(id);
    }

    public async Task<Result<Course, ValidationFailed>> Create(Course course)
    {
        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        _dbContext.Courses.Add(course);
        await _dbContext.SaveChangesAsync();

        return course;
    }

    public async Task<Result<Course?, ValidationFailed>> Update(Course course)
    {
        var validationResult = await _validator.ValidateAsync(course);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        _dbContext.Courses.Update(course);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0 ? course : default;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Courses.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
