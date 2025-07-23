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
 [HttpPost("create-company")]
    public async Task<IActionResult> CreateCompany([FromForm] CompanyCreateRequestDto request)
    {
        var command = new CompanyCreateCommand(request.CompanyName, request.Image);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
   

    [HttpGet("getCompanyById")]
    public async Task<ActionResult<Company>> GetCompanyAsync([FromQuery] CompanyDetailsQuery query)
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
    
    [HttpPut("update-company")]
    public async Task<IActionResult> UpdateCompanyAsync([FromForm] UpdateCompanyCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpDelete("delete-company")]
    public async Task<IActionResult> DeleteCompanyByCompanyIdAsync([FromQuery] DeleteCompanyCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
    
    
    /* Haircuts */
    [HttpPost("create-haircut")]
    public async Task<IActionResult> CreateHaircutAsync([FromForm] CreateHaircutCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result );
    }

    [HttpGet("get-all-haircuts-by-companyid")]
    public async Task<IActionResult> GetAllHaircutsByCompanyId([FromQuery] GetAllHaircutsByCompanyIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
    
    [HttpGet("get-haircut-details-by-id")]
    public async Task<IActionResult> GetHaircutDetailsByIdAsync([FromQuery] GetHaircutDetailsByIdQuery query)
    {
        var response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpPut("update-haircut")]
    public async Task<IActionResult> UpdateHaircutAsync([FromForm] UpdateHaircutCommand updateHaircutCommand)
    {
        var response = await Mediator.Send(updateHaircutCommand);
        return Ok(response);
    }
    
    [HttpDelete("delete-haircut")]
    public async Task<IActionResult> DeleteHaircutAsync([FromQuery] DeleteHaircutCommand deleteHaircutCommand)
    {
        var response = await Mediator.Send(deleteHaircutCommand);
        return Ok(response);
    }
}