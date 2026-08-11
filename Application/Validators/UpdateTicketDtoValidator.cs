using FluentValidation;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

namespace Technical_Assessment_ElectroPi.Application.Validators;

public class UpdateTicketDtoValidator
    : AbstractValidator<UpdateTicketDto>
{
    public UpdateTicketDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Ticket title is required.")
            .MaximumLength(250)
            .WithMessage("Ticket title cannot exceed 250 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Ticket description is required.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Invalid ticket priority.");
    }
}