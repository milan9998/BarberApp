using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Commands;

public record AssignCompanyOwnerCommand(AssignCompanyOwnerDto AssignCompanyOwnerDto):IRequest<AssignCompanyOwnerDto>;

public class AssignCompanyOwnerCommandHandler(IHairDbContext dbContext,IAuthService authService) : IRequestHandler<AssignCompanyOwnerCommand, AssignCompanyOwnerDto>
{
    public async Task<AssignCompanyOwnerDto> Handle(AssignCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var assignCompanyOwner = await 
            authService.AssignCompanyOwnerAsync(request.AssignCompanyOwnerDto, cancellationToken);
        return assignCompanyOwner;
    }
}