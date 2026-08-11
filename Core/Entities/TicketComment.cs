using Technical_Assessment_ElectroPi.Core.Common;

namespace Technical_Assessment_ElectroPi.Core.Entities;

public class TicketComment : Entity<Guid>
{
    private TicketComment()
    {
    }

    internal TicketComment(
        Guid id,
        Guid ticketId,
        Guid userId,
        string content)
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

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "Comment content is required.",
                nameof(content));

        if (content.Length > 2000)
            throw new ArgumentException(
                "Comment cannot exceed 2000 characters.",
                nameof(content));

        TicketId = ticketId;
        UserId = userId;
        Content = content.Trim();

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid TicketId { get; private set; }

    public Guid UserId { get; private set; }

    public string Content { get; private set; } = null!;
}