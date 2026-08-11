using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Contract;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<Ticket?> GetByTicketNumberAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ticket>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ticket>> GetByAssignedAgentIdAsync(
        Guid agentId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Ticket>> GetByStatusAsync(
        TicketStatus status,
        CancellationToken cancellationToken = default);
}