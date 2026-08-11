using FluentValidation;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

namespace Technical_Assessment_ElectroPi.Application.Validators;

public class AddCommentDtoValidator
    : AbstractValidator<AddCommentDto>
{
    public AddCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Comment content is required.")
            .MaximumLength(2000)
            .WithMessage("Comment cannot exceed 2000 characters.");
    }
}