using Api.Utils;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Api.Assignments
{
    public interface IAssignmentService
    {
        Task<IEnumerable<Assignment>> GetAll();
        Task<Assignment?> GetById(Guid id);
        Task<(Assignment, IEnumerable<AssignmentVariable>)?> GetByIdWithVariables(Guid id);
        Task<Result<(Assignment, IEnumerable<AssignmentVariable>), ValidationFailed>> Create(Assignment assignment, List<AssignmentVariable> variables);
        Task<Result<(Assignment, IEnumerable<AssignmentVariable>)?, ValidationFailed>> Update(Assignment assignment, List<AssignmentVariable>? variables);
        Task<bool> DeleteById(Guid id);
    }
    public class AssignmentService : IAssignmentService
    {
        protected readonly AppDbContext _dbContext;
        protected readonly IValidator<Assignment> _validator;

        public AssignmentService(AppDbContext dbContext, IValidator<Assignment> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }
        public async Task<IEnumerable<Assignment>> GetAll()
        {
            return await _dbContext.Assignments.AsNoTracking().ToListAsync();
        }

        public async Task<Assignment?> GetById(Guid id)
        {
            return await _dbContext.Assignments.FindAsync(id);
        }

        public async Task<(Assignment, IEnumerable<AssignmentVariable>)?> GetByIdWithVariables(Guid id)
        {
            var assignment = await _dbContext.Assignments.FindAsync(id);
            var variables = await _dbContext.AssignmentVariables.AsNoTracking().Where(x => x.AssignmentId == id).ToListAsync(); 
            return assignment == null || variables == null ? null : (assignment, variables);
        }

        public async Task<Result<(Assignment, IEnumerable<AssignmentVariable>), ValidationFailed>> Create(Assignment assignment, List<AssignmentVariable> variables)
        {
            var validationResult = await _validator.ValidateAsync(assignment);
            if (!validationResult.IsValid)
            {
                return new ValidationFailed(validationResult.Errors);
            }
            try
            {
                _dbContext.Assignments.Add(assignment);
                variables.ForEach(x => x.AssignmentId = assignment.Id);
                _dbContext.AssignmentVariables.AddRange(variables);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Find out what to do
                return new ValidationFailed([]);
            }

            return (assignment, variables);
        }

        public async Task<Result<(Assignment, IEnumerable<AssignmentVariable>)?, ValidationFailed>> Update(Assignment assignment, List<AssignmentVariable>? variables)
        {
            var validationResult = await _validator.ValidateAsync(assignment);
            if (!validationResult.IsValid)
            {
                return new ValidationFailed(validationResult.Errors);
            }

            var result = 0;
            try
            {
                _dbContext.Assignments.Update(assignment);
                var old_variables = await _dbContext.AssignmentVariables.AsNoTracking().Where(x => x.AssignmentId == assignment.Id).ToListAsync();
                if (variables != null) 
                {
                    _dbContext.AssignmentVariables.RemoveRange(old_variables);
                    variables.ForEach(x => x.AssignmentId = assignment.Id);
                    _dbContext.AssignmentVariables.AddRange(variables);
                }
                else
                {
                    variables = old_variables;
                }
                result = await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Find out what to do
                return new ValidationFailed([]);
            }
            return result > 0 ? (assignment, variables) : default;
        }

        public async Task<bool> DeleteById(Guid id)
        {
            var result = await _dbContext.Assignments.Where(x => x.Id == id).ExecuteDeleteAsync();
            return result > 0;
        }
    }
}
