using Hair.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Hair.Application.Auth.Queries;

public record CheckOwnerExistsQuery(Guid CompanyId): IRequest<bool>;

public class CheckOwnerExistsQueryHandler(IAuthService authService) : IRequestHandler<CheckOwnerExistsQuery, bool>
{
    public async Task<bool> Handle(CheckOwnerExistsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var checkIfExists = await authService.CheckIfCompanyOwnerExistsAsync(request.CompanyId, cancellationToken);
            return checkIfExists;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}