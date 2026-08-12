using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Contract.DTOs.Tickets;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly ICurrentUser _currentUser;

    public TicketController(
        ITicketService ticketService,
        ICurrentUser currentUser)
    {
        _ticketService = ticketService;
        _currentUser = currentUser;
    }

    // --------------------------------------------------
    // Get Ticket By Id
    // --------------------------------------------------

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,SupportAgent,Customer")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketService.GetByIdAsync(
            id,
            cancellationToken);

        if (ticket is null)
            return NotFound();

        return Ok(ticket);
    }

    // --------------------------------------------------
    // Get All Tickets
    // --------------------------------------------------

    [HttpGet]
    [Authorize(Roles = "Admin,SupportAgent,Customer")]
    public async Task<IActionResult> GetAll(
        [FromQuery] TicketQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await _ticketService.GetAllAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

    // --------------------------------------------------
    // Get Customer Tickets
    // --------------------------------------------------

    [HttpGet("customer-tickets")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCustomerTickets(
        [FromQuery] TicketQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await _ticketService.GetCustomerTicketsAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

    // --------------------------------------------------
    // Get My Tickets
    // --------------------------------------------------

    [HttpGet("my-tickets")]
    [Authorize(Roles = "SupportAgent")]
    public async Task<IActionResult> GetMyTickets(
        [FromQuery] TicketQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await _ticketService.GetMyTicketsAsync(
            query,
            cancellationToken);

        return Ok(result);
    }

    // --------------------------------------------------
    // Create Ticket
    // --------------------------------------------------

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTicketDto request,
        CancellationToken cancellationToken)
    {
        var result = await _ticketService.CreateAsync(
            request,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            result);
    }

    // --------------------------------------------------
    // Update Ticket
    // --------------------------------------------------

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateTicketDto request,
        CancellationToken cancellationToken)
    {
        await _ticketService.UpdateAsync(
            id,
            request,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Delete Ticket
    // --------------------------------------------------

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _ticketService.DeleteAsync(
            id,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Assign Agent
    // --------------------------------------------------

    [HttpPut("{ticketId:guid}/assign/{agentId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignAgent(
        Guid ticketId,
        Guid agentId,
        CancellationToken cancellationToken)
    {
        var performedByUserId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        await _ticketService.AssignAgentAsync(
            ticketId,
            agentId,
            performedByUserId,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Unassign Agent
    // --------------------------------------------------

    [HttpDelete("{ticketId:guid}/assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UnassignAgent(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var performedByUserId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        await _ticketService.UnassignAgentAsync(
            ticketId,
            performedByUserId,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Change Status
    // --------------------------------------------------

    [HttpPatch("{ticketId:guid}/status")]
    [Authorize(Roles = "Admin,SupportAgent,Customer")]
    public async Task<IActionResult> ChangeStatus(
        Guid ticketId,
        [FromQuery] TicketStatus status,
        CancellationToken cancellationToken)
    {
        var performedByUserId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        await _ticketService.ChangeStatusAsync(
            ticketId,
            status,
            performedByUserId,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Change Priority
    // --------------------------------------------------

    [HttpPatch("{ticketId:guid}/priority")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangePriority(
        Guid ticketId,
        [FromQuery] TicketPriority priority,
        CancellationToken cancellationToken)
    {
        var performedByUserId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException(
                "User is not authenticated.");

        await _ticketService.ChangePriorityAsync(
            ticketId,
            priority,
            performedByUserId,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Add Comment
    // --------------------------------------------------

    [HttpPost("{ticketId:guid}/comments")]
    [Authorize(Roles = "Admin,SupportAgent,Customer")]
    public async Task<IActionResult> AddComment(
        Guid ticketId,
        [FromBody] AddCommentDto request,
        CancellationToken cancellationToken)
    {
        await _ticketService.AddCommentAsync(
            ticketId,
            request,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Log Time
    // --------------------------------------------------

    [HttpPost("{ticketId:guid}/time-entries")]
    [Authorize(Roles = "Admin,SupportAgent")]
    public async Task<IActionResult> LogTime(
        Guid ticketId,
        [FromBody] LogTimeEntryDto request,
        CancellationToken cancellationToken)
    {
        await _ticketService.LogTimeAsync(
            ticketId,
            request,
            cancellationToken);

        return NoContent();
    }

    // --------------------------------------------------
    // Get Comments
    // --------------------------------------------------

    [HttpGet("{ticketId:guid}/comments")]
    [Authorize(Roles = "Admin,SupportAgent")]
    public async Task<IActionResult> GetComments(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var comments = await _ticketService.GetCommentsAsync(
            ticketId,
            cancellationToken);

        return Ok(comments);
    }

    // --------------------------------------------------
    // Get Time Entries
    // --------------------------------------------------

    [HttpGet("{ticketId:guid}/time-entries")]
    [Authorize(Roles = "Admin,SupportAgent")]
    public async Task<IActionResult> GetTimeEntries(
        Guid ticketId,
        CancellationToken cancellationToken)
    {
        var timeEntries = await _ticketService.GetTimeEntriesAsync(
            ticketId,
            cancellationToken);

        return Ok(timeEntries);
    }
}