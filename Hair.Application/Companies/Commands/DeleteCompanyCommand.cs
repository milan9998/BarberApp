using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Companies.Commands;

public record DeleteCompanyCommand(Guid CompanyId): IRequest<string>;


public class DeleteCompanyCommandHandler(ICompanyService companyService) : IRequestHandler<DeleteCompanyCommand, string>
{
    public async Task<string> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await companyService.DeleteCompanyByCompanyIdAsync(request.CompanyId, cancellationToken);
        return company;
    }
}