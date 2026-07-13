using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Commands;

public record VerifyEmailCommand(string UserId, string Token) : IRequest<string>;

public class VerifyEmailCommandHandler(IAuthService authService) : IRequestHandler<VerifyEmailCommand, string>
{
    public Task<string> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        return authService.VerifyEmailAsync(request.UserId, request.Token, cancellationToken);
    }
}
