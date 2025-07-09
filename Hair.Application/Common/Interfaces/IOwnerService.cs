using Hair.Application.Common.Dto.Company;

namespace Hair.Application.Common.Interfaces;

public interface IOwnerService
{
    Task<string> CreateHaircutByOwner(HaircutDto haircutDto, CancellationToken cancellationToken);
}