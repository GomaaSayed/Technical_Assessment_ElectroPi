namespace Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

public class TicketCommentDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}