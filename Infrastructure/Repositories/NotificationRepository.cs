using Microsoft.EntityFrameworkCore;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Infrastructure.Contexts;

namespace Technical_Assessment_ElectroPi.Infrastructure.Repositories;

public class NotificationRepository
    : GenericRepository<Notification>, INotificationRepository
{
    public NotificationRepository(
        TechnicalAssessmentDbContext context)
        : base(context)
    {
    }
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(
       Guid? userId,
       CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}