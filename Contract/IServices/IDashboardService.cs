using Technical_Assessment_ElectroPi.Contract.DTOs;

namespace Technical_Assessment_ElectroPi.Contract;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default);
}