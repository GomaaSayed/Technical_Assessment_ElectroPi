using Technical_Assessment_ElectroPi.Core.Common;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Core.Entities;

public class TicketActivity : Entity<Guid>
{
    private TicketActivity()
    {
    }

    internal TicketActivity(
        Guid id,
        Guid ticketId,
        Guid userId,
        TicketActivityType activityType,
        string? oldValue = null,
        string? newValue = null)
        : base(id)
    {
        if (ticketId == Guid.Empty)
            throw new ArgumentException(
                "Ticket ID cannot be empty.",
                nameof(ticketId));

        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));

        TicketId = ticketId;
        UserId = userId;
        ActivityType = activityType;
        OldValue = oldValue;
        NewValue = newValue;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid TicketId { get; private set; }

    public Guid UserId { get; private set; }

    public TicketActivityType ActivityType { get; private set; }

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }
}