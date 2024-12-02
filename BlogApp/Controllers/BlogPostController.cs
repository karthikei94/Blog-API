using Asp.Versioning;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[Controller]")]
public class BlogPostController(IBlogPostService blogPostService) : ControllerBase
{

    [HttpGet]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get()
    {
        var result = await blogPostService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await blogPostService.GetAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Post(BlogPost blogPost)
    {
        await blogPostService.CreateBlogPost(blogPost);
        return Ok();
    }
    [HttpPut("{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Put(int id, BlogPost blogPost)
    {
        await blogPostService.UpdateAsync(id, blogPost);
        return Ok();
    }

    [HttpDelete("{id}")]
    [MapToApiVersion("2.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Put(int id)
    {
        await blogPostService.DeleteAsync(id);
        return Ok();
    }
}