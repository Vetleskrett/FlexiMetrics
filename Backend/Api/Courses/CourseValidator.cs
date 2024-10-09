using Database.Models;
using FluentValidation;

namespace Api.Courses;

public class CourseValidator : AbstractValidator<Course>
{
    public CourseValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();
    }
}
