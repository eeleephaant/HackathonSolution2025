using System.Net;
using ChatService.DBModels;
using ChatService.Middleware;
using ChatService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IHasherService, HasherService>();
builder.Services.AddScoped<INotificationProducer, NotificationProducer>();
builder.Services.AddDbContext<HackatonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"))
);
builder.Services.AddSignalR();

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

builder.WebHost.ConfigureKestrel(options => { options.Listen(IPAddress.Any, 5333); });
builder.Services.AddHttpClient();


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("customPolicy");

app.UseMiddleware<ApiKeyMiddleware>();
app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot"), "uploads")),
    RequestPath = "/uploads"
});

app.Run();