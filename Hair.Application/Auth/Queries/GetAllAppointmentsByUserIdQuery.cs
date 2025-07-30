using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Auth.Queries;

public record GetAllAppointmentsByUserIdQuery(string UserId): IRequest<List<GetAllAppointmentsByUserIdDto>>;

public class GetAllAppointmentsByUserIdQueryHandler(IAuthService authService) : IRequestHandler<GetAllAppointmentsByUserIdQuery, List<GetAllAppointmentsByUserIdDto>>
{
    public async Task<List<GetAllAppointmentsByUserIdDto>> Handle(GetAllAppointmentsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var appointments = await authService.GetAllAppointmentsByUserIdAsync(request.UserId, cancellationToken);
        return appointments;
    }
}