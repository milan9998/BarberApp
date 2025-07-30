using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Commands;

public record UpdateCompanyOwnerCommand(UpdateOwnerDto UpdateOwnerDto) : IRequest<string>;

public class UpdateCompanyOwnerCommandHandler(IAuthService authService) : IRequestHandler<UpdateCompanyOwnerCommand, string>
{
    public async Task<string> Handle(UpdateCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var updatedOwner = await authService.UpdateCompanyOwnerAsync(request.UpdateOwnerDto, cancellationToken);
            return updatedOwner;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}