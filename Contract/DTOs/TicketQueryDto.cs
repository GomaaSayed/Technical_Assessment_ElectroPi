using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

public class TicketQueryDto
{
    public string? Search { get; set; }

    public TicketStatus? Status { get; set; }

    public TicketPriority? Priority { get; set; }

    public Guid? AssignedAgentId { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}