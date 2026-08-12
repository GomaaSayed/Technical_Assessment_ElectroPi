using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Technical_Assessment_ElectroPi.Contract;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ICurrentUser _currentUser;

    public NotificationHub(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = _currentUser.UserId;

        if (userId != null)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        var userId = _currentUser.UserId;

        if (userId != null)
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                $"user-{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}