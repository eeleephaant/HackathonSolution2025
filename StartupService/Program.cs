using System.Net;
using Microsoft.EntityFrameworkCore;
using StartupService.DBModels;
using StartupService.Middleware;
using StartupService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5230);
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHasherService, HasherService>();
builder.Services.AddScoped<INotificationProducer, NotificationProducer>();
builder.Services.AddScoped<IStartupService, StartupService.Services.StartupService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("customPolicy", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<HackatonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"))
);
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("customPolicy");

app.UseHttpsRedirection();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers()
    .RequireCors("customPolicy");

app.Run();