using FluentValidation;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

namespace Technical_Assessment_ElectroPi.Application.Validators;

public class LogTimeEntryDtoValidator
    : AbstractValidator<LogTimeEntryDto>
{
    public LogTimeEntryDtoValidator()
    {
        RuleFor(x => x.WorkDate)
            .NotEmpty()
            .WithMessage("Work date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Work date cannot be in the future.");

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than zero.")
            .LessThanOrEqualTo(1440)
            .WithMessage("Duration cannot exceed 24 hours.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters.");
    }
}