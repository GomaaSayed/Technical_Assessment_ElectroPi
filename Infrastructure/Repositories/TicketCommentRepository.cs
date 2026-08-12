using Microsoft.EntityFrameworkCore;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;
using Technical_Assessment_ElectroPi.Infrastructure.Contexts;

namespace Technical_Assessment_ElectroPi.Infrastructure.Repositories;

public class TicketCommentRepository
    : GenericRepository<TicketComment>, ITicketCommentRepository
{
    public TicketCommentRepository(TechnicalAssessmentDbContext context)
        : base(context)
    {
    }

}