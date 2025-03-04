using Hair.Application.Common.Exceptions;

namespace Hair.Application.Exceptions;

public class ValidationExceptions : BaseException
{
    public ValidationExceptions(IDictionary<string,string[]> failuers) : base("One or more validation failures have occurred.",failuers)
    {
        
    }
}