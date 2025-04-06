using AuthService.DBModels;
using AuthService.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services;

public interface IAuthService
{
    public Task<User?> Authenticate(LoginRequestDTO loginRequest);
    public Task<User?> Register(RegisterRequestDTO registerRequestDto);
}

public class AuthService(HackatonContext _context, IHasherService _hasherService) : IAuthService
{
    private async Task<bool> IsUserExist(string email)
    {
        return _context.Users.Any(u => u.Email == email);
    }
    
    public async Task<User?> Authenticate(LoginRequestDTO loginRequest)
    {
        
        var hashedPassword = await _hasherService.GetHash(loginRequest.Password);
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == loginRequest.Email && u.Password == hashedPassword);
        
        return user;
    }

    public async Task<User?> Register(RegisterRequestDTO registerRequestDto)
    {
        if (await IsUserExist(registerRequestDto.Email))
        {
            return null;
        }
        var hashedPassword = await _hasherService.GetHash(registerRequestDto.Password);
        var user = new User
        {
            Email = registerRequestDto.Email,
            Password = hashedPassword,
            FirstName = registerRequestDto.Name,
            LastName = registerRequestDto.Surname,
            ThirdName = registerRequestDto.Patronymic,
            Role = 1
        };
        
        await _context.Users.AddAsync(user);
        var result = await _context.SaveChangesAsync();
        
        return result > 0 ? user : null;
    }
}