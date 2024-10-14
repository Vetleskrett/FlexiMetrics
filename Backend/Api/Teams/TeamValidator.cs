using Database.Models;
using FluentValidation;

namespace Api.Teams
{
    public class TeamValidator : AbstractValidator<Team>
    {
        public TeamValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.CourseId)
                .NotEmpty();

            RuleFor(x => x.TeamId)
                .NotEmpty();
        }
    }
}
