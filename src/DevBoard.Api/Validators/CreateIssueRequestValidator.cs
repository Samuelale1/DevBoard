using DevBoard.Api.Contracts;
using FluentValidation;

namespace DevBoard.Api.Validators;

public sealed class CreateIssueRequestValidator
    : AbstractValidator<CreateIssueRequest>
{
    public CreateIssueRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Description)
            .MaximumLength(4000);

        RuleFor(x => x.ProjectId)
            .NotEmpty();

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 4);

        RuleFor(x => x.Type)
            .IsInEnum();
    }
}