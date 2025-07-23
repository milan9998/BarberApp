using Hair.Application.Common.Dto.Company;

namespace Hair.Application.Common.Interfaces;

public interface IOwnerService
{
    Task<string> CreateHaircutByOwner(HaircutDto haircutDto, CancellationToken cancellationToken);
    Task<string> DeleteHaircutByHaircutId(Guid haircutId, CancellationToken cancellationToken);
    Task<string> UpdateHaircutAsync(HaircutResponseDto haircutResponseDto, CancellationToken cancellationToken);
}