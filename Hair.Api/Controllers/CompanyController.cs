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
    

    [HttpPost("create")]
    public async Task<ActionResult<Company>> CreateCompanyAsync(CompanyCreateCommand company)
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
}