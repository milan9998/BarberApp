namespace Hair.Application.Common.Exceptions;

public class AppointmentConsecutiveException : BaseException
{
    public AppointmentConsecutiveException(string? message, object? additionalData=null) : base(message, additionalData)
    {
    }
}