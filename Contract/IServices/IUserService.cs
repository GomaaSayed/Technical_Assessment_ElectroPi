using Microsoft.AspNetCore.Identity;
using Technical_Assessment_ElectroPi.Contract.DTOs;

namespace Technical_Assessment_ElectroPi.Contract;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserDto>> GetAgentsAsync(
        CancellationToken cancellationToken = default);

    Task<IdentityResult> CreateAsync(
        CreateUserDto request,
        CancellationToken cancellationToken = default);

    Task<IdentityResult> UpdateAsync(
        string id,
        UpdateUserDto request,
        CancellationToken cancellationToken = default);

    Task<IdentityResult> DeleteAsync(
        string id,
        CancellationToken cancellationToken = default);
}