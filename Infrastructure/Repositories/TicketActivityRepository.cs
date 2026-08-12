using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Infrastructure.Contexts;

namespace Technical_Assessment_ElectroPi.Infrastructure.Repositories;

public class TicketActivityRepository
    : GenericRepository<TicketActivity>, ITicketActivityRepository
{
    public TicketActivityRepository(
        TechnicalAssessmentDbContext context)
        : base(context)
    {
    }
}