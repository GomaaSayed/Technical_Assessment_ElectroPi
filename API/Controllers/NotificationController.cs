using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Technical_Assessment_ElectroPi.Contract;

namespace Technical_Assessment_ElectroPi.API.Controllers;

[Authorize(Roles = "SupportAgent")]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(
        INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        CancellationToken cancellationToken)
    {
        var notifications =
            await _notificationService.GetByUserIdAsync(
                );

        return Ok(notifications);
    }
}