using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Companies.Commands;

public record UpdateCompanyCommand(UpdateCompanyDto UpdateCompanyDto) : IRequest<string>;

public class UpdateCompanyCommandHandler(ICompanyService companyService) : IRequestHandler<UpdateCompanyCommand, string>
{
    public async Task<string> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var companyForUpdate = await companyService.UpdateCompanyAsync(request.UpdateCompanyDto, cancellationToken);
        return companyForUpdate;
    }
}