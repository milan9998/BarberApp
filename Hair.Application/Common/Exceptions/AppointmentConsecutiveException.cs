namespace Hair.Application.Common.Exceptions;

public class AppointmentConsecutiveException : BaseException
{
    public AppointmentConsecutiveException(string? message, object? additionalData) : base(message, additionalData)
    {
    }
}