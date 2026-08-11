using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Infrastructure.Contexts;

namespace Technical_Assessment_ElectroPi.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly TechnicalAssessmentDbContext _context;

    public UnitOfWork(TechnicalAssessmentDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}