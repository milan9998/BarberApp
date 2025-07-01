using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Commands;

public record LoginCommand(LoginDto LoginDto) : IRequest<AuthLevelDto>;

public class LoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, AuthLevelDto?>
{
    public async Task<AuthLevelDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var login = await authService.Login(request.LoginDto, cancellationToken);
        
        return login;
    }
}