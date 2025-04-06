using System.Net;
using AIChatService.Services;
using ChatService.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<IHasherService, HasherService>();

builder.WebHost.ConfigureKestrel(options => { options.Listen(IPAddress.Any, 6012); });
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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("customPolicy");
app.UseMiddleware<ApiKeyMiddleware>();
app.UseHttpsRedirection();

app.MapControllers().RequireCors("customPolicy");

app.Run();