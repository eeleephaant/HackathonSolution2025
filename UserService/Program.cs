using System.Net;
using UserService.Services;
using Microsoft.EntityFrameworkCore;
using UserService.DBModels;
using UserService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5228);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IHasherService, UserService.Services.HasherService>();
builder.Services.AddScoped<IUserService, UserService.Services.UserService>();

builder.Services.AddDbContext<HackatonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyMiddleware>();
app.UseCors("customPolicy");
app.UseHttpsRedirection();

app.MapControllers().RequireCors("customPolicy");

app.Run();