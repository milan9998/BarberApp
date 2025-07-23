using Hair.Application.Common.Dto.Auth;
using Hair.Application.Common.Dto.Company;

namespace Hair.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> Login(LoginDto loginDto, CancellationToken cancellationToken);

    Task<List<CompanyDetailsDto>> GetCompaniesByOwnerEmailAsync(string email, CancellationToken cancellationToken);
    Task<AuthLevelDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);

    Task<CompanyOwnerResponseDto> CreateCompanyOwnerAsync(CompanyOwnerDto companyOwnerDto,
        CancellationToken cancellationToken);

    Task<bool> CheckIfCompanyOwnerExistsAsync(Guid companyId, CancellationToken cancellationToken);

    Task<AssignCompanyOwnerDto> AssignCompanyOwnerAsync(AssignCompanyOwnerDto assignCompanyOwnerDto,
        CancellationToken cancellationToken);
}