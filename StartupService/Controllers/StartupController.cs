using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using StartupService.DBModels;
using StartupService.Models;
using StartupService.Services;
using UserService.Middleware;

namespace StartupService.Controllers;

[ApiController]
[Route("startup")]
public class StartupController(IStartupService _startupService, INotificationProducer _notificationProducer) : ControllerBase
{
    [HttpPost]
    public async ValueTask<IActionResult> AddNew([FromBody] NewStartupDTO newStartup)
    {
        var user = HttpContext.Items["User"] as User;
        if (user.RoleNavigation.Name != Roles.Startupper.Name)
        {
            return BadRequest(new { message = "You are not a startupper" });
        }

        var result = await _startupService.AddNew(newStartup, user);
        return result ? Ok(result) : BadRequest(new { message = "Something went wrong" });
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("{startupId}")]
    public async ValueTask<IActionResult> GetStartup([FromRoute] int startupId)
    {
        var user = HttpContext.Items["User"] as User;
        
        var startup = await _startupService.GetStartup(startupId);
        if (startup == null)
            return BadRequest(new { message = "Startup not found" });
        
        return Ok(startup);
    }    
    
    [HttpGet]
    [AllowAnonymous]
    public async ValueTask<IActionResult> GetStartup()
    {
        var user = HttpContext.Items["User"] as User;
        
        var startups = await _startupService.GetStartups();
        
        return Ok(startups);
    }

    [HttpPut]
    [Route("{startupId}/workers")]
    public async ValueTask<IActionResult> AddWorker([FromRoute] int startupId, [FromBody] NewStartupBuddy user1)
    {
        var user = HttpContext.Items["User"] as User;
        
        if (!user.Startups.Any(s => s.Id == startupId)) 
            return BadRequest(new { message = "You are not a startupper" });
        
        var result = await _startupService.AddWorker(user1.Id, startupId);

        if (result)
        {
            await _notificationProducer.SendNotificationAsync(new NotificationDto
            {
                Body = "Скорее знакомьтесь со всеми",
                Title = "Вас наняли в стартап!",
                UserId = user1.Id,
            });
        }
        
        return result ? Ok() : BadRequest(new { message = "Something went wrong" });
    }    
    
    [HttpPut]
    [Route("{startupId}/scientists")]
    public async ValueTask<IActionResult> AddScientist([FromRoute] int startupId, [FromBody] NewStartupBuddy user1)
    {
        var user = HttpContext.Items["User"] as User;
        
        if (!user.Startups.Any(s => s.Id == startupId)) 
            return BadRequest(new { message = "You are not a startupper" });
        
        
        
        var result = await _startupService.AddScientist(user1.Id, startupId);
        
        if (result)
        {
            await _notificationProducer.SendNotificationAsync(new NotificationDto
            {
                Body = "Скорее знакомьтесь со всеми",
                Title = "Вас наняли в стартап!",
                UserId = user1.Id,
            });
        }
        return result ? Ok() : BadRequest(new { message = "Something went wrong" });
    }    
    
    [HttpPut]
    [Route("{startupId}/investors")]
    public async ValueTask<IActionResult> AddInvestor([FromRoute] int startupId, [FromBody] NewStartupBuddy user1)
    {
        var user = HttpContext.Items["User"] as User;
        
        if (!user.Startups.Any(s => s.Id == startupId)) 
            return BadRequest(new { message = "You are not a startupper" });
        
        var result = await _startupService.AddInvestor(user1.Id, startupId);
        
        if (result)
        {
            await _notificationProducer.SendNotificationAsync(new NotificationDto
            {
                Body = "Скорее знакомьтесь со всеми",
                Title = "Вас наняли в стартап!",
                UserId = user1.Id,
            });
        }
        
        return result ? Ok() : BadRequest(new { message = "Something went wrong" });
    }

    [HttpDelete]
    [Route("{startupId}/users")]
    public async ValueTask<IActionResult> RemoveUserFromStartup([FromRoute] int startupId, [FromBody] NewStartupBuddy user1)
    {
        var user = HttpContext.Items["User"] as User;
        if (!user.Startups.Any(s => s.Id == startupId))
        {
            return BadRequest(new { message = "You are not a startupper" });
        }
        var result = await _startupService.RemoveUserFromStartup(startupId, user1.Id);
        if (result)
        {
            await _notificationProducer.SendNotificationAsync(new NotificationDto
            {
                Body = "Вас удалили из стартапа :(",
                Title = "Грустная новость",
                UserId = user1.Id,
            });
        }
        
        return result ? Ok() : BadRequest(new { message = "Something went wrong" });
    }
    
}