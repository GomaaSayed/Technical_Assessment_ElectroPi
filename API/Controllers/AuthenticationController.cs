using Technical_Assessment_ElectroPi.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;

namespace Technical_Assessment_ElectroPi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            var user = new User()
            {
                UserName = model.Username,
                Email = model.Email,
                Password = model.Password,
                EmailConfirmed = true
            };
            var result = await _authService.RegisterAsync(user);
            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully." });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            var user = new User()
            {
                UserName = model.Username,
                Password = model.Password,

            };
            var token = await _authService.LoginAsync(user);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid login attempt." });
            }
            return Ok(new { token });
        }
    }

}
