using Asp.Versioning;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[Controller]")]
[ApiController]
public class FileUploadController(IFileUploadService fileUploadService) : ControllerBase
{
    [HttpPost]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        var result = await fileUploadService.UploadFile(file, userId);
        return Ok(result);
    }
}