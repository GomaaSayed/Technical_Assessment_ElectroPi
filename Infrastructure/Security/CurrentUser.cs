using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Technical_Assessment_ElectroPi.Contract;

namespace Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?
                .User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(value, out var userId))
            {
                throw new UnauthorizedAccessException(
                    "User is not authenticated.");
            }

            return userId;
        }
    }

    public string? UserName =>
        _httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User
            .Identity?
            .IsAuthenticated == true;

    public bool IsInRole(string role) =>
        _httpContextAccessor.HttpContext?
            .User
            .IsInRole(role) == true;
}