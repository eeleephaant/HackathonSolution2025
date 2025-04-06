using System.Reflection;
using UserService.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using UserService.DBModels;

namespace UserService.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
{
    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { message = "Invalid API Key" });
            await context.Response.CompleteAsync();
            return;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var _context = scope.ServiceProvider.GetRequiredService<HackatonContext>();
            var _hasher = scope.ServiceProvider.GetRequiredService<IHasherService>();
            var apiKeyString = extractedApiKey.ToString();
            string hash = await _hasher.GetHash(apiKeyString);
            var userSession = await _context.UserSessions.Include(userSession => userSession.User).FirstOrDefaultAsync(us => us.Token == hash);

            if (userSession == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message = "Invalid API Key" });
                return;
            }
            
            context.Request.Headers.TryGetValue("X-Forwarded-For", out var ip);

            userSession.LastAccessedIp = ip;
            userSession.LastAccessedAt = DateTime.UtcNow;

            context.Items["User"] = userSession.User;
            context.Items["UserSession"] = userSession;

            await next(context);
        }
    }
}