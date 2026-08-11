using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;
using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly ICurrentUser _currentUser;

    public TicketService(
        ITicketRepository ticketRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser)
    {
        _ticketRepository = ticketRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<TicketDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketRepository.GetByIdAsync(
            id,
            cancellationToken);

        return ticket is null
            ? null
            : MapToDto(ticket);
    }

    public async Task<PagedResultDto<TicketDto>> GetAllAsync(
        TicketQueryDto query,
        CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetAllAsync(
            cancellationToken);

        IEnumerable<Ticket> filteredTickets = tickets;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            filteredTickets = filteredTickets.Where(x =>
                x.TicketNumber.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ||
                x.Title.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase) ||
                x.Description.Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (query.Status.HasValue)
        {
            filteredTickets = filteredTickets.Where(
                x => x.Status == query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            filteredTickets = filteredTickets.Where(
                x => x.Priority == query.Priority.Value);
        }

        if (query.AssignedAgentId.HasValue)
        {
            filteredTickets = filteredTickets.Where(
                x => x.AssignedAgentId == query.AssignedAgentId.Value);
        }

        filteredTickets = query.SortBy?.ToLowerInvariant() switch
        {
            "title" => query.Descending
                ? filteredTickets.OrderByDescending(x => x.Title)
                : filteredTickets.OrderBy(x => x.Title),

            "priority" => query.Descending
                ? filteredTickets.OrderByDescending(x => x.Priority)
                : filteredTickets.OrderBy(x => x.Priority),

            "status" => query.Descending
                ? filteredTickets.OrderByDescending(x => x.Status)
                : filteredTickets.OrderBy(x => x.Status),

            "createdat" => query.Descending
                ? filteredTickets.OrderByDescending(x => x.CreatedAt)
                : filteredTickets.OrderBy(x => x.CreatedAt),

            _ => filteredTickets.OrderByDescending(x => x.CreatedAt)
        };

        var totalCount = filteredTickets.Count();

        var pageNumber = Math.Max(1, query.PageNumber);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var items = filteredTickets
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResultDto<TicketDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TicketDto> CreateAsync(
        CreateTicketDto request,
        CancellationToken cancellationToken = default)
    {
        var ticketNumber = GenerateTicketNumber();

        // TODO:
        // Get CustomerId from the authenticated user.
        var customerId = _currentUser.UserId
        ?? throw new UnauthorizedAccessException();

        var ticket = Ticket.Create(
            ticketNumber,
            request.Title,
            request.Description,
            customerId,
            request.Priority);

        await _ticketRepository.AddAsync(
            ticket,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return MapToDto(ticket);
    }

    public async Task UpdateAsync(
        Guid id,
        UpdateTicketDto request,
        CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            id,
            cancellationToken);

        ticket.UpdateDetails(
            request.Title,
            request.Description);

        if (ticket.Priority != request.Priority)
        {
            // TODO:
            // Get current user ID from ICurrentUser.
            var performedByUserId = _currentUser.UserId
      ?? throw new UnauthorizedAccessException();
            ticket.ChangePriority(
                request.Priority,
                performedByUserId);
        }

        _ticketRepository.Update(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            id,
            cancellationToken);

        _ticketRepository.Delete(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AssignAgentAsync(
        Guid ticketId,
        Guid agentId,
        Guid performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            ticketId,
            cancellationToken);

        ticket.AssignAgent(
            agentId,
            performedByUserId);

        _ticketRepository.Update(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task UnassignAgentAsync(
        Guid ticketId,
        Guid performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            ticketId,
            cancellationToken);

        ticket.UnassignAgent(
            performedByUserId);

        _ticketRepository.Update(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task ChangeStatusAsync(
        Guid ticketId,
        TicketStatus status,
        Guid performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            ticketId,
            cancellationToken);

        ticket.ChangeStatus(
            status,
            performedByUserId);

        _ticketRepository.Update(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task ChangePriorityAsync(
        Guid ticketId,
        TicketPriority priority,
        Guid performedByUserId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            ticketId,
            cancellationToken);

        ticket.ChangePriority(
            priority,
            performedByUserId);

        _ticketRepository.Update(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    public async Task AddCommentAsync(
      Guid ticketId,
      AddCommentDto request,
      CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            ticketId,
            cancellationToken);

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        ticket.AddComment(
            userId,
            request.Content);

        _ticketRepository.Update(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
    public async Task LogTimeAsync(
      Guid ticketId,
      LogTimeEntryDto request,
      CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketAsync(
            ticketId,
            cancellationToken);

        var userId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        ticket.LogTime(
            userId,
            request.WorkDate,
            request.DurationMinutes,
            request.Description);

        _ticketRepository.Update(ticket);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    private async Task<Ticket> GetTicketAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(
            id,
            cancellationToken);

        if (ticket is null)
        {
            throw new KeyNotFoundException(
                $"Ticket with ID '{id}' was not found.");
        }

        return ticket;
    }

    private static TicketDto MapToDto(Ticket ticket)
    {
        return new TicketDto
        {
            Id = ticket.Id,
            TicketNumber = ticket.TicketNumber,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            Priority = ticket.Priority,
            CustomerId = ticket.CustomerId,
            AssignedAgentId = ticket.AssignedAgentId,
            ResolvedAt = ticket.ResolvedAt,
            ClosedAt = ticket.ClosedAt,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            TotalTimeInMinutes = ticket.GetTotalTimeInMinutes(),

            Comments = ticket.Comments
                .Select(x => new TicketCommentDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Content = x.Content,
                    CreatedAt = x.CreatedAt
                })
                .ToList(),

            Activities = ticket.Activities
                .Select(x => new TicketActivityDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    ActivityType = x.ActivityType,
                    OldValue = x.OldValue,
                    NewValue = x.NewValue,
                    CreatedAt = x.CreatedAt
                })
                .ToList()
        };
    }

    private static string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
}