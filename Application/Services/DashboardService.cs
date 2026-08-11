using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly ITicketRepository _ticketRepository;

    public DashboardService(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<DashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetAllAsync(
            cancellationToken);

        var totalTickets = tickets.Count;

        var openTickets = tickets.Count(
            x => x.Status == TicketStatus.Open);

        var inProgressTickets = tickets.Count(
            x => x.Status == TicketStatus.InProgress);

        var resolvedTickets = tickets.Count(
            x => x.Status == TicketStatus.Resolved);

        var closedTickets = tickets.Count(
            x => x.Status == TicketStatus.Closed);

        var openCriticalTickets = tickets.Count(
            x =>
                x.Status != TicketStatus.Closed &&
                x.Priority == TicketPriority.Critical);

        var resolvedTicketsWithTime = tickets
            .Where(x =>
                x.ResolvedAt.HasValue &&
                x.ResolvedAt.Value >= x.CreatedAt)
            .ToList();

        var averageResolutionTimeInMinutes =
            resolvedTicketsWithTime.Count == 0
                ? 0
                : resolvedTicketsWithTime
                    .Average(x =>
                        (x.ResolvedAt!.Value - x.CreatedAt)
                        .TotalMinutes);

        var agentWorkloads = tickets
            .Where(x => x.AssignedAgentId.HasValue)
            .GroupBy(x => x.AssignedAgentId!.Value)
            .Select(group => new AgentWorkloadDto
            {
                AgentId = group.Key,
                AssignedTickets = group.Count()
            })
            .ToList();

        return new DashboardDto
        {
            TotalTickets = totalTickets,
            OpenTickets = openTickets,
            InProgressTickets = inProgressTickets,
            ResolvedTickets = resolvedTickets,
            ClosedTickets = closedTickets,
            OpenCriticalTickets = openCriticalTickets,
            AverageResolutionTimeInMinutes =
                averageResolutionTimeInMinutes,
            AgentWorkloads = agentWorkloads
        };
    }
}