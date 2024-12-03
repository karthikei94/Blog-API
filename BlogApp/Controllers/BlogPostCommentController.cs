using Asp.Versioning;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[Controller]")]
public class BlogPostCommentController(IPostCommentService commentService) : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var result = await commentService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("GetByPostId/{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await commentService.GetByPostAsync(id);
        return Ok(result);
    }
    
    [HttpPost]
    [MapToApiVersion("2.0")]
    [Route("AddComment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Post(PostComments comment)
    {
        await commentService.AddComment(comment);
        return Ok();
    }

    [HttpPost("LikeComment/{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Post(int id)
    {
        await commentService.LikeComment(id);
        return Ok();
    }
}
