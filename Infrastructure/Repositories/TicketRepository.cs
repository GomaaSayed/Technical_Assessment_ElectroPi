using Microsoft.EntityFrameworkCore;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;
using Technical_Assessment_ElectroPi.Infrastructure.Contexts;

namespace Technical_Assessment_ElectroPi.Infrastructure.Repositories;

public class TicketRepository
    : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(TechnicalAssessmentDbContext context)
        : base(context)
    {
    }

    public async Task<Ticket?> GetByTicketNumberAsync(
        string ticketNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.TicketNumber == ticketNumber,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Ticket>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ticket>> GetByAssignedAgentIdAsync(
        Guid agentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.AssignedAgentId == agentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ticket>> GetByStatusAsync(
        TicketStatus status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Status == status)
            .ToListAsync(cancellationToken);
    }
}