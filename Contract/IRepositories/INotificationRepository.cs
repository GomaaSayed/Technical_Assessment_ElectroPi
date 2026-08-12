using Technical_Assessment_ElectroPi.Core.Entities;

namespace Technical_Assessment_ElectroPi.Contract;

public interface INotificationRepository
    : IGenericRepository<Notification>
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(
        Guid? userId,
        CancellationToken cancellationToken = default);

}