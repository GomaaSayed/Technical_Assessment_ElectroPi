using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

public class TicketDto
{
    public Guid Id { get; set; }

    public string TicketNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public TicketStatus Status { get; set; }

    public TicketPriority Priority { get; set; }

    public Guid CustomerId { get; set; }

    public Guid? AssignedAgentId { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int TotalTimeInMinutes { get; set; }

    public IReadOnlyList<TicketCommentDto> Comments { get; set; } = [];

    public IReadOnlyList<TicketActivityDto> Activities { get; set; } = [];
    public IReadOnlyList<LogTimeEntryDto> TimeEntries { get; set; } = [];

}