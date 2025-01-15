using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Movies.Api.Contracts.Responses;

namespace Api.Teams;

public interface ITeamService
{
    Task<IEnumerable<Team>> GetAll();
    Task<Team?> GetById(Guid id);
    Task<Result<Team, ValidationResponse>> Create(Team team);
    Task<Result<Team?, ValidationResponse>> Update(Team team);
    Task<bool> DeleteById(Guid id);
}

public class TeamService : ITeamService
{
    protected readonly AppDbContext _dbContext;
    protected readonly IValidator<Team> _validator;

    public TeamService(AppDbContext dbContext, IValidator<Team> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }
    public async Task<IEnumerable<Team>> GetAll()
    {
        return await _dbContext.Teams.AsNoTracking().ToListAsync();
    }

    public async Task<Team?> GetById(Guid id)
    {
        return await _dbContext.Teams.FindAsync(id);
    }

    public async Task<Result<Team, ValidationResponse>> Create(Team team)
    {
        var validationResult = await _validator.ValidateAsync(team);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync();

        return team;
    }

    public async Task<Result<Team?, ValidationResponse>> Update(Team team)
    {
        var validationResult = await _validator.ValidateAsync(team);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.MapToResponse();
        }

        _dbContext.Teams.Update(team);
        var result = await _dbContext.SaveChangesAsync();

        return result > 0 ? team : default;
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.Teams.Where(x => x.Id == id).ExecuteDeleteAsync();
        return result > 0;
    }
}
