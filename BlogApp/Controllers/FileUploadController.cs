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
    public async Task<Uri> UploadFile(IFormFile file)
    {
        var result = await fileUploadService.UploadFile(file.FileName, file);
        return result;
    }
}