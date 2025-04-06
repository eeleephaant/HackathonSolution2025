using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(ITokenService _tokenService, IAuthService _authService, ISessionService _sessionService)
    : ControllerBase
{
    [HttpPost("register")]
    public async ValueTask<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        string userAgent = Request.Headers["User-Agent"].ToString();
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(userAgent) || string.IsNullOrEmpty(ipAddress))
        {
            return BadRequest("All fields are required");
        }


        var user = await _authService.Register(request);

        if (user == null)
        {
            return BadRequest(new { message = "Something went wrong" });
        }

        var newToken = await _tokenService.GenerateToken();

        var result = await _sessionService.CreateSession(newToken, ipAddress, userAgent, user);
        return result ? Ok(new { token = newToken }) : BadRequest(new { message = "Error while creating session" });
    }

    [HttpPost("login")]
    public async ValueTask<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        string userAgent = Request.Headers["User-Agent"].ToString();
        string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(request.Email) ||
            !System.Text.RegularExpressions.Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return BadRequest(new { message = "Invalid email format" });
        }


        var user = await _authService.Authenticate(request);

        if (user == null)
        {
            return BadRequest(new { message = "User not exists" });
        }

        var newToken = await _tokenService.GenerateToken();


        var result = await _sessionService.CreateSession(newToken, ipAddress, userAgent, user);
        if (!result)
        {
            return BadRequest(new { message = "Failed to create session" });
        }

        return Ok(new { token = newToken });
    }
}