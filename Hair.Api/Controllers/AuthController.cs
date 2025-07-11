﻿using Hair.Application.Auth.Commands;
using Hair.Application.Auth.Queries;
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
}
