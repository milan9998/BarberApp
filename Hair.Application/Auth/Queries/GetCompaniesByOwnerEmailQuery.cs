using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Queries;

public record GetCompaniesByOwnerEmailQuery(string Email): IRequest<List<CompanyDetailsDto>>;

public class GetCompaniesByOwnerEmailQueryHandler(IAuthService authService) : IRequestHandler<GetCompaniesByOwnerEmailQuery, List<CompanyDetailsDto>>
{
    public async Task<List<CompanyDetailsDto>> Handle(GetCompaniesByOwnerEmailQuery request, CancellationToken cancellationToken)
    {
        var response = await authService.GetCompaniesByOwnerEmailAsync(request.Email, cancellationToken);
        return response;
    }
}