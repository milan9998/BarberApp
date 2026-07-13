using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Commands;

public record RegisterCommand(RegisterDto Register): IRequest<RegisterResponseDto>;

public class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterCommand, RegisterResponseDto?>
{
    public async Task<RegisterResponseDto?> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var register = await authService.RegisterAsync(request.Register, cancellationToken);
        return register;
    }
}