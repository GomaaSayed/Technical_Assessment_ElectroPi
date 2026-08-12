namespace Technical_Assessment_ElectroPi.Core.Entities;

public class Notification
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Message { get; private set; } = null!;

    public string Type { get; private set; } = null!;

    public Guid? ReferenceId { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Notification()
    {
    }

    public Notification(
        Guid userId,
        string title,
        string message,
        string type,
        Guid? referenceId = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        ReferenceId = referenceId;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}