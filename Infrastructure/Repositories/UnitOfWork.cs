using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Core.Entities;
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
        _context.ChangeTracker.DetectChanges();

        foreach (var entry in _context.ChangeTracker.Entries())
        {
            Console.WriteLine(
                $"ENTITY: {entry.Entity.GetType().Name} | " +
                $"STATE: {entry.State}");

            if (entry.Entity is Ticket ticket)
            {
                var original = entry.Property(nameof(Ticket.RowVersion))
                    .OriginalValue as byte[];

                var current = entry.Property(nameof(Ticket.RowVersion))
                    .CurrentValue as byte[];

                Console.WriteLine(
                    $"RowVersion Original: {Convert.ToHexString(original ?? [])}");

                Console.WriteLine(
                    $"RowVersion Current: {Convert.ToHexString(current ?? [])}");
            }
        }

        return _context.SaveChangesAsync(cancellationToken);
    }
}