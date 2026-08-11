using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;

namespace Technical_Assessment_ElectroPi.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(
            cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(
            id,
            cancellationToken);

        if (user is null)
        {
            return NotFound(new
            {
                message = "User not found."
            });
        }

        return Ok(user);
    }

    [HttpGet("agents")]
    public async Task<IActionResult> GetAgents(
        CancellationToken cancellationToken)
    {
        var agents = await _userService.GetAgentsAsync(
            cancellationToken);

        return Ok(agents);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserDto request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.CreateAsync(
            request,
            cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new
        {
            message = "User created successfully."
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateUserDto request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(
            id,
            cancellationToken);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return NoContent();
    }
}