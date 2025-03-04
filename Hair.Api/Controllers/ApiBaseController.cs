using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hair.Api.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class ApiBaseController : ControllerBase
{
    private ISender? _mediator;
    public ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    
}