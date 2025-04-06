using AuthService.DBModels;

namespace AuthService.Services;

public interface ISessionService
{
    public Task<bool> CreateSession(string token, string ip, string userAgent, User user);
}

public class SessionService(HackatonContext _context, IHasherService _hasherService) : ISessionService
{
    public async Task<bool> CreateSession(string token, string ip, string userAgent, User user)
    {
        var hashedToken = await _hasherService.GetHash(token);
        await _context.UserSessions.AddAsync(new UserSession
        {
            Token = hashedToken,
            UserId = user.Id,
            LastAccessedIp = ip,
            LastAccessedUserAgent = userAgent,
            Id = Guid.NewGuid()
        });
        return await _context.SaveChangesAsync() > 0;
    }
}