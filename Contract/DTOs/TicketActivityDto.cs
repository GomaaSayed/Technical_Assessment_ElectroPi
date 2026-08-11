using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

public class TicketActivityDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public TicketActivityType ActivityType { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime CreatedAt { get; set; }
}