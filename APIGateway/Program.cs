using System.Net;
using APIGatewayHackaton.Middleware;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .ConfigureHttpClient((_, handler) =>
    {
        handler.AllowAutoRedirect = false;
    });

builder.Services.Configure<ForwarderRequestConfig>(config =>
{
    config = new ForwarderRequestConfig
    {
        ActivityTimeout = TimeSpan.FromMinutes(30)
    };
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 8000);
});




var app = builder.Build();
app.UseCors("customPolicy");

app.UseMiddleware<TokenForwardingMiddleware>();



app.MapReverseProxy().RequireCors("customPolicy");

app.Run();