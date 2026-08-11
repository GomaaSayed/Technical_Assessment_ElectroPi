namespace Technical_Assessment_ElectroPi.Contract.DTOs;

public class DashboardDto
{
    public int TotalTickets { get; set; }

    public int OpenTickets { get; set; }

    public int InProgressTickets { get; set; }

    public int ResolvedTickets { get; set; }

    public int ClosedTickets { get; set; }

    public int OpenCriticalTickets { get; set; }

    public double AverageResolutionTimeInMinutes { get; set; }

    public IReadOnlyList<AgentWorkloadDto> AgentWorkloads { get; set; }
        = [];
}