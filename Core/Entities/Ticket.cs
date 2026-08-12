using Technical_Assessment_ElectroPi.Core.Common;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;
using Technical_Assessment_ElectroPi.Core.Entities.Exceptions;

namespace Technical_Assessment_ElectroPi.Core.Entities;

public class Ticket : AggregateRoot<Guid>
{
    private readonly List<TicketComment> _comments = [];
    private readonly List<TicketActivity> _activities = [];
    private readonly List<TimeEntry> _timeEntries = [];

    private Ticket()
    {
    }

    private Ticket(
        Guid id,
        string ticketNumber,
        string title,
        string description,
        Guid customerId,
        TicketPriority priority)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber))
            throw new ArgumentException(
                "Ticket number is required.",
                nameof(ticketNumber));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Ticket title is required.",
                nameof(title));

        if (title.Length > 250)
            throw new ArgumentException(
                "Ticket title cannot exceed 250 characters.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Ticket description is required.",
                nameof(description));

        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));

        TicketNumber = ticketNumber.Trim();
        Title = title.Trim();
        Description = description.Trim();

        CustomerId = customerId;

        Status = TicketStatus.Open;
        Priority = priority;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public string TicketNumber { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public TicketStatus Status { get; private set; }

    public TicketPriority Priority { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid? AssignedAgentId { get; private set; }

    public DateTime? ResolvedAt { get; private set; }

    public DateTime? ClosedAt { get; private set; }

    public byte[] RowVersion { get; private set; } = [];

    public IReadOnlyCollection<TicketComment> Comments =>
        _comments.AsReadOnly();

    public IReadOnlyCollection<TicketActivity> Activities =>
        _activities.AsReadOnly();

    public IReadOnlyCollection<TimeEntry> TimeEntries =>
        _timeEntries.AsReadOnly();

    public static Ticket Create(
        string ticketNumber,
        string title,
        string description,
        Guid customerId,
        TicketPriority priority = TicketPriority.Medium)
    {
        return new Ticket(
            Guid.NewGuid(),
            ticketNumber,
            title,
            description,
            customerId,
            priority);
    }
    public void UpdateDetails(
    string title,
    string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Ticket title is required.",
                nameof(title));

        if (title.Length > 250)
            throw new ArgumentException(
                "Ticket title cannot exceed 250 characters.",
                nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Ticket description is required.",
                nameof(description));

        Title = title.Trim();
        Description = description.Trim();

        SetUpdatedAt();
    }
    public TicketActivity AssignAgent(
        Guid agentId,
        Guid performedByUserId)
    {
        if (agentId == Guid.Empty)
            throw new ArgumentException(
                "Agent ID cannot be empty.",
                nameof(agentId));

        if (performedByUserId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(performedByUserId));

        var oldAgentId = AssignedAgentId;

        AssignedAgentId = agentId;

        var activity = AddActivity(
            performedByUserId,
            TicketActivityType.AgentAssigned,
            oldAgentId?.ToString(),
            agentId.ToString());

        SetUpdatedAt();

        return activity;
    }

    public TicketActivity? UnassignAgent(Guid performedByUserId)
    {
        if (performedByUserId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(performedByUserId));

        if (AssignedAgentId is null)
            return null;

        var oldAgentId = AssignedAgentId;

        AssignedAgentId = null;

        var activity = AddActivity(
            performedByUserId,
            TicketActivityType.AgentUnassigned,
            oldAgentId?.ToString(),
            null);

        SetUpdatedAt();

        return activity;
    }

    public void ChangePriority(
        TicketPriority priority,
        Guid performedByUserId)
    {
        if (performedByUserId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(performedByUserId));

        if (Priority == priority)
            return;

        var oldPriority = Priority;

        Priority = priority;

        AddActivity(
            performedByUserId,
            TicketActivityType.PriorityChanged,
            oldPriority.ToString(),
            priority.ToString());

        SetUpdatedAt();
    }

    public TicketActivity ChangeStatus(
        TicketStatus newStatus,
        Guid performedByUserId)
    {
        if (performedByUserId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(performedByUserId));

        if (Status == newStatus)
            throw new InvalidOperationException(
                "Ticket already has this status.");

        ValidateStatusTransition(Status, newStatus);

        var oldStatus = Status;

        Status = newStatus;

        if (newStatus == TicketStatus.Resolved)
        {
            ResolvedAt = DateTime.UtcNow;
        }

        if (newStatus == TicketStatus.Closed)
        {
            ClosedAt = DateTime.UtcNow;
        }

        var activity = AddActivity(
            performedByUserId,
            TicketActivityType.StatusChanged,
            oldStatus.ToString(),
            newStatus.ToString());

        SetUpdatedAt();

        return activity;
    }

    public void Resolve(Guid performedByUserId)
    {
        ChangeStatus(
            TicketStatus.Resolved,
            performedByUserId);
    }

    public void Close(Guid performedByUserId)
    {
        ChangeStatus(
            TicketStatus.Closed,
            performedByUserId);
    }

    public TicketComment AddComment(
        Guid userId,
        string content)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "Comment content cannot be empty.",
                nameof(content));

        var comment = new TicketComment(
            Guid.NewGuid(),
            Id,
            userId,
            content);

        SetUpdatedAt();

        return comment;
    }

    public TimeEntry LogTime(
      Guid userId,
      DateTime workDate,
      int durationMinutes,
      string description)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException(
                "User ID cannot be empty.",
                nameof(userId));

        if (durationMinutes <= 0)
            throw new ArgumentException(
                "Duration must be greater than zero.",
                nameof(durationMinutes));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Description cannot be empty.",
                nameof(description));

        var timeEntry = new TimeEntry(
            Guid.NewGuid(),
            Id,
            userId,
            workDate,
            durationMinutes,
            description);

        SetUpdatedAt();

        return timeEntry;
    }

    public int GetTotalTimeInMinutes()
    {
        return _timeEntries.Sum(x => x.DurationMinutes);
    }

    private TicketActivity AddActivity(
      Guid userId,
      TicketActivityType activityType,
      string? oldValue = null,
      string? newValue = null)
    {
        var activity = new TicketActivity(
            Guid.NewGuid(),
            Id,
            userId,
            activityType,
            oldValue,
            newValue);

        _activities.Add(activity);

        return activity;
    }

    private static void ValidateStatusTransition(
        TicketStatus currentStatus,
        TicketStatus requestedStatus)
    {
        var isValid = currentStatus switch
        {
            TicketStatus.Open =>
                requestedStatus == TicketStatus.InProgress,

            TicketStatus.InProgress =>
                requestedStatus == TicketStatus.Open ||
                requestedStatus == TicketStatus.Resolved,

            TicketStatus.Resolved =>
                requestedStatus == TicketStatus.Closed,

            TicketStatus.Closed => false,

            _ => false
        };

        if (!isValid)
        {
            throw new InvalidTicketStatusTransitionException(
                currentStatus,
                requestedStatus);
        }
    }
}