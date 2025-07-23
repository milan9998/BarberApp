using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Barbers.Commands;

public record DeleteBarberCommand(Guid BarberId) : IRequest<string>;

public class DeleteBarberCommandHandler(IBarberService barberService) : IRequestHandler<DeleteBarberCommand, string>
{
    public async Task<string> Handle(DeleteBarberCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var barberToDelete = await barberService.DeleteBarberAsync(request.BarberId, cancellationToken);
            return barberToDelete;
        }
        catch (Exception e)
        {
            throw new Exception($"Error deleting barber: {e.Message}");
        }
        
    }
}