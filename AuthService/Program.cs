using System.Net;
using System.Text;
using AuthService.DBModels;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// DI for services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService.Services.AuthService>();
builder.Services.AddScoped<IHasherService, HasherService>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<HackatonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"))
);

builder.WebHost.ConfigureKestrel(options => { options.Listen(IPAddress.Any, 5223); });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("customPolicy");

app.MapControllers().RequireCors("customPolicy");

app.Run();