using ChatService.Models;
using Microsoft.AspNetCore.Mvc;
using ChatService.DBModels;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Controllers;

[ApiController]
[Route("messages")]
public class MessagesController(HackatonContext context) : ControllerBase
{
    [HttpGet("startup/{startupId}")]
    public async Task<IActionResult> GetStartupMessages(int startupId)
    {
        var messages = await context.Messages
            .Include(m => m.Sender)
            .Where(m => m.StartupId == startupId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    
        var result = messages.Select(m => new MessageDTO
        {
            Id = m.Id,
            StartupId = m.StartupId,
            SenderId = m.SenderId,
            Text = m.Text,
            AttachmentUrl = m.AttachmentUrl,
            AttachmentType = m.AttachmentType,
            SentAt = m.SentAt,
            Sender = new UserDTO
            {
                Id = m.Sender.Id,
                FirstName = m.Sender.FirstName,
                LastName = m.Sender.LastName,
                ThirdName = m.Sender.ThirdName
            }
        }).ToList();
    
        return Ok(result);
    }
}