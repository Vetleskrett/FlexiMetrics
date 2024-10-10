using Api.Utils;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Api.Templates
{
    public interface IService<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(Guid id);
        Task<Result<T, ValidationFailed>> Create(T instance);
        Task<Result<T?, ValidationFailed>> Update(T instance);
        Task<bool> DeleteById(Guid id);
    }
    public abstract class Service<T> where T : IEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly IValidator<T> _validator;

        public Service(AppDbContext dbContext, IValidator<T> validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }
        public async Task<IEnumerable<T>> GetAll()
        {
            return await GetDbSet().AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetById(Guid id)
        {
            return await GetDbSet().FindAsync(id);
        }

        public async Task<Result<T, ValidationFailed>> Create(T instance)
        {
            var validationResult = await _validator.ValidateAsync(instance);
            if (!validationResult.IsValid)
            {
                return new ValidationFailed(validationResult.Errors);
            }
            try
            {
                GetDbSet().Add(instance);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception) 
            {
                // Find out what to do
                return new ValidationFailed([]);
            }

            return instance;
        }

        public async Task<Result<T?, ValidationFailed>> Update(T instance)
        {
            var validationResult = await _validator.ValidateAsync(instance);
            if (!validationResult.IsValid)
            {
                return new ValidationFailed(validationResult.Errors);
            }

            var result = 0;
            try
            {
                GetDbSet().Update(instance);
                result = await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Find out what to do
                return new ValidationFailed([]);
            }
            return result > 0 ? instance : default;
        }

        public async Task<bool> DeleteById(Guid id)
        {
            var result = await GetDbSet().Where(x => x.Id == id).ExecuteDeleteAsync();
            return result > 0;
        }

        private DbSet<T> GetDbSet()
        {
            return _dbContext.Set<T>();
        }
    }
}
