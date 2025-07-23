using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Companies.Commands;

public record UpdateHaircutCommand(HaircutResponseDto HaircutResponseDto): IRequest<string>;

public class UpdateHaircutCommandHandler(IOwnerService ownerService) : IRequestHandler<UpdateHaircutCommand, string>
{
    public async Task<string> Handle(UpdateHaircutCommand request, CancellationToken cancellationToken)
    {
        var haircutForUpdate = await ownerService.UpdateHaircutAsync(request.HaircutResponseDto, cancellationToken);
        return haircutForUpdate;
    }
}