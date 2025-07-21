namespace Hair.Application.Common.Exceptions;

public class AppointmentConflictException : BaseException
{
    public AppointmentConflictException(string? message, object? additionalData = null) : base(message, additionalData)
    {
    }
}