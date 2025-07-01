using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Commands;

public record RegisterCommand(RegisterDto RegisterDto): IRequest<AuthLevelDto>;

public class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterCommand, AuthLevelDto?>
{
    public async Task<AuthLevelDto?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var register = await authService.RegisterAsync(request.RegisterDto, cancellationToken);
        return register;
    }
}