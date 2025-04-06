using Microsoft.AspNetCore.Mvc;
using UserService.DBModels;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("user")]
public class UserController(IUserService _userService) : ControllerBase
{
    [HttpGet("{userId}")]
    public async ValueTask<ActionResult> GetUser([FromRoute] int userId)
    {
        return Ok(await _userService.GetUser(userId));
    }
    
    [HttpPatch("{userId}")]
    public async ValueTask<ActionResult> UpdateUser([FromBody] UserUpdateDTO userUpdateDTO, [FromRoute] int userId)
    {
        var fromUser = HttpContext.Items["User"] as User;
        if (fromUser.Id != userId)
        {
            return BadRequest(new { message = "You can't update this user" });
        }

        if (await _userService.GetRoleId(userUpdateDTO.Role) == null)
        {
            return BadRequest(new { message = "Role not exists" });
        }

        var result = await _userService.UpdateUserProfile(userId, userUpdateDTO);

        if (!result)
        {
            return BadRequest(new { message = "Internal Server Error" });
        }

        return Ok(new { message = "User updated successfully" });
    }

    [HttpPut("{userId}/skills")]
    public async ValueTask<ActionResult> AddSkill([FromRoute] int userId, [FromBody] SkillInputDTO skillInputId)
    {
        var fromUser = HttpContext.Items["User"] as User;
        if (fromUser.Id != userId)
            return BadRequest(new { message = "You can't add skill to this user" });
        
        var result = await _userService.AddSkill(userId, skillInputId.Id);
        if (!result)
            return BadRequest(new { message = "Something went wrong" });
        return Ok();
    }
    
    [HttpGet("/skills")]
    public async ValueTask<ActionResult<List<SkillDTO>>> GetSkills()
    {
        return Ok(await _userService.GetSkills());
    }
    
    [HttpDelete("{userId}/skills")]
    public async ValueTask<ActionResult> RemoveSkill([FromRoute] int userId, [FromBody] SkillInputDTO skillInputId)
    {
        var fromUser = HttpContext.Items["User"] as User;
        if (fromUser.Id != userId)
            return BadRequest(new { message = "You can't add skill to this user" });
        
        var result = await _userService.RemoveSkill(userId, skillInputId.Id);
        if (!result)
            return BadRequest(new { message = "Something went wrong" });
        return Ok();
    }

    [HttpPost("fcm_token")]
    public async ValueTask<ActionResult> UpdateFCMToken([FromBody] FCMUpdateDTO fcmUpdateDTO)
    {
        var user = HttpContext.Items["User"] as User;
        var result = await _userService.UpdateFCM(fcmUpdateDTO, user.Id);
        if (result)
            return BadRequest("Something went wrong");
        return Ok();
    }
}