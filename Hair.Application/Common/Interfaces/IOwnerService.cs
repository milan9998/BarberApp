using Hair.Application.Common.Dto.Company;

namespace Hair.Application.Common.Interfaces;

public interface IOwnerService
{
    Task<HaircutDto> CreateHaircutByOwner(HaircutDto haircutDto, CancellationToken cancellationToken);
}