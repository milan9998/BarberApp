using Hair.Application.Common.Exceptions;

namespace Hair.Application.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string? message, object? additionalData) : base(message, additionalData)
    {
    }
}