using Microsoft.AspNetCore.SignalR;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _icurrentUser;

    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext,
        IUnitOfWork unitOfWork, ICurrentUser icurrentUser)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _unitOfWork = unitOfWork;
        _icurrentUser = icurrentUser; ;
    }

    public async Task SendToUserAsync(
        Guid userId,
        string title,
        string message,
        string type,
        Guid? referenceId = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification(
            userId,
            title,
            message,
            type,
            referenceId);

        await _notificationRepository.AddAsync(
            notification,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        await _hubContext.Clients
     .Group($"user-{userId}")
     .SendAsync(
         "ReceiveNotification",
         new
         {
             notification.Id,
             notification.Title,
             notification.Message,
             notification.Type,
             notification.ReferenceId,
             notification.IsRead,
             notification.CreatedAt
         },
         cancellationToken);
    }
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        CancellationToken cancellationToken = default)
    {
        var userId = _icurrentUser.UserId;

        return await _notificationRepository.GetByUserIdAsync(
            userId,
            cancellationToken);
    }
}