using Microsoft.AspNetCore.Mvc;
using NotificationService.Models;

namespace NotificationService.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationController(Services.NotificationService _notificationService) : ControllerBase
{
    [HttpPost]
    [Route("send-notification")]
    public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequest request) 
    {
        var result = await _notificationService.SendNotificationsAsync(request);
        if (result)
        {
            return Ok(new { message = "Notification sent successfully" });
        }
        return BadRequest();
    }
}