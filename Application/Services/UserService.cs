using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace Technical_Assessment_ElectroPi.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(
        UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var result = new List<UserDto>();

        foreach (var user in users)
        {
            result.Add(await MapToDtoAsync(user));
        }

        return result;
    }

    public async Task<UserDto?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        if (user is null)
            return null;

        return await MapToDtoAsync(user);
    }

    public async Task<IReadOnlyList<UserDto>> GetAgentsAsync(
        CancellationToken cancellationToken = default)
    {
        var agents = await _userManager.GetUsersInRoleAsync(
            "SupportAgent");

        var result = new List<UserDto>();

        foreach (var agent in agents)
        {
            result.Add(await MapToDtoAsync(agent));
        }

        return result;
    }

    public async Task<IdentityResult> CreateAsync(
        CreateUserDto request,
        CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
            return result;

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var roleResult = await _userManager.AddToRoleAsync(
                user,
                request.Role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return roleResult;
            }
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(
        string id,
        UpdateUserDto request,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found."
                });
        }

        if (!string.IsNullOrWhiteSpace(request.Username) &&
            user.UserName != request.Username)
        {
            var usernameResult = await _userManager.SetUserNameAsync(
                user,
                request.Username);

            if (!usernameResult.Succeeded)
                return usernameResult;
        }

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            user.Email != request.Email)
        {
            var emailResult = await _userManager.SetEmailAsync(
                user,
                request.Email);

            if (!emailResult.Succeeded)
                return emailResult;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(
                user);

            var passwordResult =
                await _userManager.ResetPasswordAsync(
                    user,
                    token,
                    request.Password);

            if (!passwordResult.Succeeded)
                return passwordResult;
        }

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (!string.IsNullOrWhiteSpace(request.Role) &&
            !currentRoles.Contains(request.Role))
        {
            var removeRolesResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    currentRoles);

            if (!removeRolesResult.Succeeded)
                return removeRolesResult;

            var addRoleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    request.Role);

            if (!addRoleResult.Succeeded)
                return addRoleResult;
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return IdentityResult.Failed(
                new IdentityError
                {
                    Code = "UserNotFound",
                    Description = "User not found."
                });
        }

        return await _userManager.DeleteAsync(user);
    }

    private async Task<UserDto> MapToDtoAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return new UserDto
        {
            Id = user.Id,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles.ToList()
        };
    }
}