using Hair.Application.Common.Exceptions;
using Hair.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hair.Api.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    // Mapa tipova exceptiona i metoda koji ih obrađuju
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilterAttribute()
    {
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationExceptions), HandleValidationException },
            { typeof(FluentValidation.ValidationException), HandleFluentValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(AppointmentConflictException), HandleAppointmentConflictException },
            { typeof(AppointmentConsecutiveException), HandleAppointmentConsecutiveException }
        };
    }

    // Ovo je JEDINI OnException metod u klasi
    public override void OnException(ExceptionContext context)
    {
        Console.WriteLine("Exception caught in filter: " + context.Exception.GetType().Name);
        HandleException(context);
    }

    // Odlučuje koji handler da pozove na osnovu tipa exceptiona
    private void HandleException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();
        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            handler.Invoke(context);
            return;
        }

        
        HandleUnknownException(context);
    }

    //
    private void HandleAppointmentConflictException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Title = "Sukob termina",
            Detail = context.Exception.Message,
            Status = StatusCodes.Status409Conflict
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status409Conflict
        };

        context.ExceptionHandled = true;
    }

   
    private void HandleUnknownException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }

   
    private void HandleFluentValidationException(ExceptionContext context)
    {
        var exception = (FluentValidation.ValidationException)context.Exception;
        var failures = exception.Errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());

        var details = new ValidationProblemDetails(failures)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

  
    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;

        var details = new ProblemDetails()
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        };

        context.Result = new NotFoundObjectResult(details);
        context.ExceptionHandled = true;
    }

   
    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };

        context.ExceptionHandled = true;
    }

   
    private void HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationExceptions)context.Exception;

        var details = new ValidationProblemDetails((IDictionary<string, string[]>)exception.AdditionalData!)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

 
    
}
