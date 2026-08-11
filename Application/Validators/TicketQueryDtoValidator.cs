using FluentValidation;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

namespace Technical_Assessment_ElectroPi.Application.Validators;

public class TicketQueryDtoValidator
    : AbstractValidator<TicketQueryDto>
{
    public TicketQueryDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Invalid ticket status.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue)
            .WithMessage("Invalid ticket priority.");
    }
}