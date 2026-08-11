namespace Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;

public class LogTimeEntryDto
{
    public DateTime WorkDate { get; set; }

    public int DurationMinutes { get; set; }

    public string Description { get; set; } = null!;
}