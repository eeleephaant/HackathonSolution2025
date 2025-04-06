namespace APIGatewayHackaton.Middleware;

public class TokenForwardingMiddleware
{
    private readonly RequestDelegate _next;

    public TokenForwardingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var clientIp = context.Connection.RemoteIpAddress?.ToString();

        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers["Authorization"] = $"{token}";
        }

        if (!string.IsNullOrEmpty(clientIp))
        {
            context.Request.Headers["X-Forwarded-For"] = clientIp;
        }

        await _next(context);
    }
}