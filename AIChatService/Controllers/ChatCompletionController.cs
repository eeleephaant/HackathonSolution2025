using AIChatService.Models;
using AIChatService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIChatService.Controllers;

[ApiController]
[Route("ai")]
public class ChatCompletionController(IAiService _aiService) : ControllerBase
{
    [HttpPost("chat")]
    public async ValueTask<ActionResult<ResponseDTO>> GetCompletion([FromBody] PromptDTO prompt) 
    {
        var result = await _aiService.GetCompletion(prompt.Text);
        return Ok(new ResponseDTO{ Response = result});
    }
    
}