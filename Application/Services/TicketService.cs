using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;
using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITicketActivityRepository _ticketActivityRepository;
    private readonly ITicketCommentRepository _ticketCommentRepository;
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly INotificationService _notificationService;
    public TicketService(
       ITicketRepository ticketRepository,
       ITicketActivityRepository ticketActivityRepository,
       ITicketCommentRepository ticketCommentRepository,
       ITimeEntryRepository timeEntryRepository,
       IUnitOfWork unitOfWork,
       ICurrentUser currentUser,
       INotificationService notificationService
      )
    {
        _ticketRepository = ticketRepository;
        _ticketActivityRepository = ticketActivityRepository;
        _ticketCommentRepository = ticketCommentRepository;
        _timeEntryRepository = timeEntryRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _notificationService = notificationService;
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

    public async Task<PagedResultDto<TicketDto>> GetCustomerTicketsAsync(
     TicketQueryDto query,
     CancellationToken cancellationToken = default)
    {
        var customerId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException();

        var tickets = await _ticketRepository.GetAllAsync(
            cancellationToken);

        // Only tickets created by the current customer
        IEnumerable<Ticket> filteredTickets = tickets
            .Where(x => x.CustomerId == customerId);

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

            "ticketnumber" => query.Descending
                ? filteredTickets.OrderByDescending(x => x.TicketNumber)
                : filteredTickets.OrderBy(x => x.TicketNumber),

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
    public async Task<PagedResultDto<TicketDto>> GetMyTicketsAsync(
    TicketQueryDto query,
    CancellationToken cancellationToken = default)
    {
        var agentId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException();

        var tickets = await _ticketRepository.GetAllAsync(
            cancellationToken);

        // Only tickets assigned to the current support agent
        IEnumerable<Ticket> filteredTickets = tickets
            .Where(x => x.AssignedAgentId == agentId);

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
    public async Task<IReadOnlyList<TicketCommentDto>> GetCommentsAsync(
    Guid ticketId,
    CancellationToken cancellationToken = default)
    {
        var comments = await _ticketCommentRepository.GetAllAsync(
            cancellationToken);

        return comments
            .Where(x => x.TicketId == ticketId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new TicketCommentDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Content = x.Content,
                CreatedAt = x.CreatedAt
            })
            .ToList();
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

        var activity = ticket.AssignAgent(
            agentId,
            performedByUserId);

        await _ticketActivityRepository.AddAsync(
            activity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        await _notificationService.SendToUserAsync(
            agentId,
            "New Ticket Assigned",
            $"Ticket #{ticket.TicketNumber} has been assigned to you.",
            "TicketAssigned",
            ticket.Id,
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

        var activity = ticket.UnassignAgent(
            performedByUserId);

        if (activity is not null)
        {
          await  _ticketActivityRepository.AddAsync(activity);
        }

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

        var activity = ticket.ChangeStatus(
            status,
            performedByUserId);

        await _ticketActivityRepository.AddAsync(
            activity,
            cancellationToken);

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

        var comment = ticket.AddComment(
            userId,
            request.Content);

        await _ticketCommentRepository.AddAsync(
            comment,
            cancellationToken);

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

        var timeEntry = ticket.LogTime(
            userId,
            request.WorkDate,
            request.DurationMinutes,
            request.Description);

        await _timeEntryRepository.AddAsync(
            timeEntry,
            cancellationToken);

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
    public async Task<IReadOnlyList<LogTimeEntryDto>> GetTimeEntriesAsync(
 Guid ticketId,
 CancellationToken cancellationToken = default)
    {
        var timeEntries = await _timeEntryRepository.GetAllAsync(
            cancellationToken);

        return timeEntries
            .Where(x => x.TicketId == ticketId)
            .OrderByDescending(x => x.WorkDate)
            .Select(x => new LogTimeEntryDto
            {

                WorkDate = x.WorkDate,
                DurationMinutes = x.DurationMinutes,
                Description = x.Description,

            })
            .ToList();

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