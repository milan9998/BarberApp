using Hair.Application.Common.Interfaces;
using Hair.Application.Schedules.Commands;
using Hair.Application.Schedules.Queries;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hair.Api.Controllers;
[ApiController]
[Route("schedule")]
public class ScheduleController: ApiBaseController
{
    [HttpPost ("CreateAppointment")]
    public async Task<ActionResult<Appointment>> CreateAppointmentAsync([FromBody]ScheduleAppointmentCommand command)
    {
        try
        {
            var result = await Mediator.Send(command);
            
            return Ok(new { Message = "Successfully scheduled appointment.", Data = result });
        }
        catch (Exception ex)
        {
           
            return BadRequest(new { Message = ex.Message });
        }
     
    }
    
    [HttpGet ("GetAllUsedAppointments")]
    public async Task<ActionResult<Appointment>> GetAllUsedAppointmentsAsync([FromQuery]GetAllScheduledAppointments query)
    {
        return Ok(await Mediator.Send(query));
    }

    
    [HttpGet ("GetAllFreeAppointments")]
    public async Task<ActionResult<Appointment>> GetAllFreeAppointmentsAsync([FromQuery]GetAllFreeAppointmentsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    
}