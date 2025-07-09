using System.Net;
using Hair.Application.Barbers.Queries;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Application.Companies;
using Hair.Application.Companies.Commands;
using Hair.Application.Companies.Queries;
using Hair.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Messaging;

namespace Hair.Api.Controllers;


[ApiController]
[Route("company")]
public class CompanyController(IHairDbContext dbContext): ApiBaseController
{
    
/*
    [HttpPost("create")]
    public async Task<ActionResult<Company>> CreateCompanyAsync([FromForm]CompanyCreateCommand company)
    {
        try
        {
            var result = await Mediator.Send(company);
            return Ok(new { Message = "Company created", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = "Error", Data = ex.Message });
        }
        
    }*/

    [HttpPost("create-company")]
    public async Task<IActionResult> CreateCompany([FromForm] CompanyCreateRequestDto request)
    {
        var command = new CompanyCreateCommand(request.CompanyName, request.Image);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    
    [HttpGet("getCompanyById")]
    public async Task<ActionResult<Company>> GetCompanyByIdAsync([FromQuery] CompanyDetailsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("getAllCompanies")]
    public async Task<ActionResult<List<Company>>> GetAllCompaniesAsync()
    {
        return Ok(await Mediator.Send(new GetAllCompaniesQuery(CompanyDetailsDto: new CompanyDetailsDto())));
    }
    [HttpGet("getCompanyDetailsById")]
    public async Task<ActionResult<Company>> GetCompanyDetailsByIdAsync([FromQuery] CompanyDetailsByIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("create-haircut")]
    public async Task<IActionResult> CreateHaircut([FromForm] CreateHaircutCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = result });
        }
        catch (Exception e)
        {
            return BadRequest(new { Message = e.Message });
        }
        //return Ok(await Mediator.Send(command));
    }
    [HttpGet("get-all-haircuts-by-companyid")]
    public async Task<IActionResult> GetAllHaircutsByCompanyId([FromQuery] GetAllHaircutsByCompanyIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
    
    
}