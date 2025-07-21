using Hair.Application.Auth.Commands;
using Hair.Application.Auth.Queries;
using Hair.Application.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hair.Api.Controllers;
[ApiController]
[Route("auth")]
public class AuthController : ApiBaseController
{
    [HttpGet("test/conflict")]
    public IActionResult TestConflict()
    {
        throw new AppointmentConflictException("Test sukob termina.");
    }
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

    [HttpPost("createCompanyOwner")]
    public async Task<IActionResult> CreateCompanyOwner([FromForm] CreateCompanyOwnerCommand createCompanyOwnerCommand)
    {
        return Ok(await Mediator.Send(createCompanyOwnerCommand));
    }

    [HttpGet("checkIfCompanyOwnerExists")]
    public async Task<IActionResult> CheckIfCompanyOwnerExists([FromQuery] CheckOwnerExistsQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    [HttpGet("get-owners")]
    public async Task<IActionResult> GetOwners([FromQuery] GetAllOwnersQuery query, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    [HttpPost("AssignCompanyOwner")]
    public async Task<IActionResult> AssignCompanyOwner([FromForm] AssignCompanyOwnerCommand assignCompanyOwnerCommand)
    {
        return Ok(await Mediator.Send(assignCompanyOwnerCommand));
    }
    
}
