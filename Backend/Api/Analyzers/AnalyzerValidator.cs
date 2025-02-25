using Database.Models;
using FluentValidation;

namespace Api.Analyzers;

public class AnalyzerValidator : AbstractValidator<Analyzer>
{
    public AnalyzerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(name => name.EndsWith(".py"))
            .WithMessage("Filename must end with .py");

        RuleFor(x => x.AssignmentId)
            .NotEmpty();
    }
}
