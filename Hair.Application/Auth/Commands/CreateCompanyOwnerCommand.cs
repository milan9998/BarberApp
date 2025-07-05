using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Domain.Enums;
using MediatR;

namespace Hair.Application.Auth.Commands;

public record CreateCompanyOwnerCommand(CompanyOwnerDto CompanyOwnerDto):IRequest<CompanyOwnerResponseDto>;

public class CreateCompanyOwnerCommandHandler(IHairDbContext dbContext,IAuthService authService) : IRequestHandler<CreateCompanyOwnerCommand, CompanyOwnerResponseDto>
{
    public async Task<CompanyOwnerResponseDto> Handle(CreateCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var companyOwner = await authService.CreateCompanyOwnerAsync(request.CompanyOwnerDto, cancellationToken);
        return companyOwner;
    }
}