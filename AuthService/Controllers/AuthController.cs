using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ITokenService _tokenService, IAuthService _authService) : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDTO request)
    {
        if (request.Email == "admin" && request.Password == "1234")
        {
            var token = _tokenService.GenerateToken(request.Email);
            return Ok(new { token });
        }

        return Unauthorized();
    }
}