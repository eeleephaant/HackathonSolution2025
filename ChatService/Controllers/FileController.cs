using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;


[ApiController]
[Route("files")]
public class FileController(IWebHostEnvironment env) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required");

        var uploadsDir = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads");

        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{uniqueFileName}";

        return Ok(new { url = fileUrl });
    }
}