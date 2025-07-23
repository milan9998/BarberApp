using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Barbers.Commands;

public record UpdateBarberCommand(UpdateBarberDto UpdateBarberDto) : IRequest<string>;

public class UpdateBarberCommandHandler(IBarberService barberService) : IRequestHandler<UpdateBarberCommand, string>
{
    public async Task<string> Handle(UpdateBarberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var barberToUpdate = await barberService.UpdateBarberAsync(request.UpdateBarberDto, cancellationToken);
            return barberToUpdate;
        }
        catch (Exception e)
        {
            throw new Exception($"Error occured while updating barber: {e.Message}");
        }
        
    }
}