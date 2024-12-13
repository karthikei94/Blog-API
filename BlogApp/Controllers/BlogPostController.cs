using Asp.Versioning;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[Controller]")]
public class BlogPostController(IBlogPostService blogPostService) : ControllerBase
{

    [HttpGet]
    [MapToApiVersion("2.0")]
    [Route("GetAll")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var result = await blogPostService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet]
    [MapToApiVersion("2.0")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagedResult([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] bool createdByMe)
    {
        var user = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        var result = await blogPostService.GetPagedResultsAsync(page, pageSize, user ?? string.Empty, createdByMe);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [MapToApiVersion("2.0")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(string id)
    {
        var result = await blogPostService.GetAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Post(BlogPost blogPost)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        await blogPostService.CreateBlogPost(blogPost, userId);
        return Ok();
    }
    [HttpPut("{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Put(string id, BlogPost blogPost)
    {
        await blogPostService.UpdateAsync(id, blogPost);
        return Ok();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Put(string id)
    {
        await blogPostService.DeleteAsync(id);
        return Ok();
    }

    [HttpPost("LikePost/{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> LikePost(string id)
    {
        await blogPostService.LikePost(id);
        return Ok();
    }
}