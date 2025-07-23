using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Companies.Commands;

public record DeleteHaircutCommand(Guid HaircutId) : IRequest<string>;

public class DeleteHaircutCommandHandler(IOwnerService ownerService) : IRequestHandler<DeleteHaircutCommand, string>
{
    public async Task<string> Handle(DeleteHaircutCommand request, CancellationToken cancellationToken)
    {
        var haircutForDelete = await ownerService.DeleteHaircutByHaircutId(request.HaircutId, cancellationToken);
        return haircutForDelete;
    }
}