using Technical_Assessment_ElectroPi.Core.Common;

namespace Technical_Assessment_ElectroPi.Core.Entities;

public class TimeEntry : Entity<Guid>
{
    private TimeEntry()
    {
    }

    internal TimeEntry(
        Guid id,
        Guid ticketId,
        Guid userId,
        DateTime workDate,
        int durationMinutes,
        string description)
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

        if (durationMinutes <= 0)
            throw new ArgumentException(
                "Duration must be greater than zero.",
                nameof(durationMinutes));

        if (durationMinutes > 1440)
            throw new ArgumentException(
                "Duration cannot exceed 24 hours.",
                nameof(durationMinutes));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Description is required.",
                nameof(description));

        if (description.Length > 1000)
            throw new ArgumentException(
                "Description cannot exceed 1000 characters.",
                nameof(description));

        TicketId = ticketId;
        UserId = userId;
        WorkDate = workDate;
        DurationMinutes = durationMinutes;
        Description = description.Trim();

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public Guid TicketId { get; private set; }

    public Guid UserId { get; private set; }

    public DateTime WorkDate { get; private set; }

    public int DurationMinutes { get; private set; }

    public string Description { get; private set; } = null!;
}