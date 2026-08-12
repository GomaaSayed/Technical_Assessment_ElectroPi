using Microsoft.AspNetCore.Identity;
using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace Technical_Assessment_ElectroPi.Contract;

public interface INotificationService
{
    Task SendToUserAsync(
        Guid userId,
        string title,
        string message,
        string type,
        Guid? referenceId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        CancellationToken cancellationToken = default);
}