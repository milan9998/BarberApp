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
    [HttpGet("get-barber-details-by-barber-id")]
    public async Task<IActionResult> GetBarberDetailsByBarberIdAsync([FromQuery] GetBarberDetailsByBarberId query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPut("update-barber-details")]
    public async Task<IActionResult> UpdateBarberAsync([FromForm] UpdateBarberCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    [HttpDelete("delete-barber")]
    public async Task<IActionResult> DeleteBarberAsync([FromQuery] DeleteBarberCommand deleteBarberCommand)
    {
        var result = await Mediator.Send(deleteBarberCommand);
        return Ok(result);
    }
}