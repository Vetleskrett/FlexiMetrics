using Api.Courses;
using Api.Templates;
using Api.Utils;
using Api.Validation;
using Database;
using Database.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Api.Teams
{
    public class TeamService : Service<Team>
    {
        public TeamService(AppDbContext dbContext, IValidator<Team> validator) : base(dbContext, validator)
        {
        }
    }
}
