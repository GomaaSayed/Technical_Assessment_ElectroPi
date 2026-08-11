namespace Technical_Assessment_ElectroPi.Contract;

public interface ICurrentUser
{
    Guid? UserId { get; }

    string? UserName { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(string role);
}