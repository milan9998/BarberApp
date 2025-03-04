using Hair.Application.Barbers.Commands;
using Hair.Application.Barbers.Queries;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hair.Api.Controllers;

[ApiController]
[Route("barber")]
public class BarberController(IHairDbContext dbContext) : ApiBaseController
{
    [HttpPost("create")]
    public async Task<ActionResult<Barber>> CreateBarberAsync(BarberCreateCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            return Ok(new { Message = "Barber created", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
       
    }
    
    [HttpGet("getAllBarbersByCompanyId")]
    public async Task<ActionResult<Barber>> GetAllBarbersByCompanyIdAsync([FromQuery] GetAllBarbersQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
    
}