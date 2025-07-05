using Hair.Application.Auth.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hair.Api.Controllers;
[ApiController]
[Route("auth")]
public class AuthController : ApiBaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginCommand loginCommand)
    {
        return Ok(await Mediator.Send(loginCommand));
    }
    
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromForm] RegisterCommand registerCommand)
    {
        return Ok(await Mediator.Send(registerCommand));
    }
}
