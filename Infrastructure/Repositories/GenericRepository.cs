using Microsoft.EntityFrameworkCore;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Infrastructure.Contexts;

namespace Technical_Assessment_ElectroPi.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    private readonly TechnicalAssessmentDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(TechnicalAssessmentDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(
            new object[] { id },
            cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        _dbSet.Remove(entity);
    }
}