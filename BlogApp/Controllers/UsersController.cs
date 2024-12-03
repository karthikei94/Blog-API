using Asp.Versioning;
using BlogApp.Models;
using BlogApp.Services;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("2.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
public class UsersController(IAuthenticationService authService) : ControllerBase
{

    // [HttpPost("Login")]
    // public async Task<IActionResult> Login([FromBody] UserLoginModel userCreationModel)
    // {
    //     var result = await authService.Login(userCreationModel.UserName, userCreationModel.Password);
    //     return Ok(result);
    // }

    [HttpPost("SignUp")]
    public async Task<IActionResult> SignUp([FromBody] UserSignupModel userModel)
    {
        var result = await authService.SignUp(userModel.DisplayName, userModel.UserName, userModel.Password);
        return Ok(new { Uid = result });
    }
}
