using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace Technical_Assessment_ElectroPi.Core.Entities.Exceptions;

public class InvalidTicketStatusTransitionException : Exception
{
    public InvalidTicketStatusTransitionException(
        TicketStatus currentStatus,
        TicketStatus requestedStatus)
        : base(
            $"Invalid ticket status transition from " +
            $"'{currentStatus}' to '{requestedStatus}'.")
    {
        CurrentStatus = currentStatus;
        RequestedStatus = requestedStatus;
    }

    public TicketStatus CurrentStatus { get; }

    public TicketStatus RequestedStatus { get; }
}