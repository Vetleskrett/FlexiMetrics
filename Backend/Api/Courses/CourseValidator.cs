using Database.Models;
using FluentValidation;

namespace Api.Courses;

public class CourseValidator : AbstractValidator<Course>
{
    public CourseValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.Year)
            .NotEmpty();

        RuleFor(x => x.Semester)
            .IsInEnum();
    }
}
