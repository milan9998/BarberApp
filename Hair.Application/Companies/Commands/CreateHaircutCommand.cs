using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using MediatR;

namespace Hair.Application.Companies.Commands;

public record CreateHaircutCommand(HaircutDto HaircutDto): IRequest<string>;

public class CreateHaircutCommandHandler(IOwnerService ownerService) : IRequestHandler<CreateHaircutCommand, string>
{
    public async Task<string> Handle(CreateHaircutCommand request, CancellationToken cancellationToken)
    {
        var haircut = await ownerService.CreateHaircutByOwner(request.HaircutDto, cancellationToken);
        return haircut;
    }
}