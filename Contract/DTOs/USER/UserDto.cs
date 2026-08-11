namespace Technical_Assessment_ElectroPi.Contract.DTOs;

public class UserDto
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public IReadOnlyList<string> Roles { get; set; } = [];
}