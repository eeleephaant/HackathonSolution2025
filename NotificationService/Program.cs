using System.Net;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using NotificationService.DBModels;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();

builder.Services.AddDbContext<HackatonContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgres"))
);

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hackathon-preview-app-firebase-adminsdk-fbsvc-41a57ee5dd.json")),
});

builder.WebHost.ConfigureKestrel(options => { options.Listen(IPAddress.Any, 5444); });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers()
    .RequireCors("customPolicy");

app.Run();