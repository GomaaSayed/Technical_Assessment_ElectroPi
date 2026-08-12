using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Contract;

public interface ITicketService
{
    Task<TicketDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<PagedResultDto<TicketDto>> GetMyTicketsAsync(
   TicketQueryDto query,
   CancellationToken cancellationToken = default);
    Task<PagedResultDto<TicketDto>> GetCustomerTicketsAsync(
    TicketQueryDto query,
    CancellationToken cancellationToken = default);
    Task<PagedResultDto<TicketDto>> GetAllAsync(
        TicketQueryDto query,
        CancellationToken cancellationToken = default);

    Task<TicketDto> CreateAsync(
        CreateTicketDto request,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        Guid id,
        UpdateTicketDto request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task AssignAgentAsync(
        Guid ticketId,
        Guid agentId,
        Guid performedByUserId,
        CancellationToken cancellationToken = default);

    Task UnassignAgentAsync(
        Guid ticketId,
        Guid performedByUserId,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        Guid ticketId,
        TicketStatus status,
        Guid performedByUserId,
        CancellationToken cancellationToken = default);

    Task ChangePriorityAsync(
        Guid ticketId,
        TicketPriority priority,
        Guid performedByUserId,
        CancellationToken cancellationToken = default);

    Task AddCommentAsync(
        Guid ticketId,
        AddCommentDto request,
        CancellationToken cancellationToken = default);

    Task LogTimeAsync(
        Guid ticketId,
        LogTimeEntryDto request,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketCommentDto>> GetCommentsAsync(
    Guid ticketId,
    CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LogTimeEntryDto>> GetTimeEntriesAsync(
    Guid ticketId,
    CancellationToken cancellationToken = default);
}