using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

public class UpdateTicketDto
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TicketPriority Priority { get; set; }
}