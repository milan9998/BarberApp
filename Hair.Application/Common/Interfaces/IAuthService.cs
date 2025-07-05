using Hair.Application.Common.Dto.Auth;

namespace Hair.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthLevelDto> Login(LoginDto loginDto, CancellationToken cancellationToken);
    
    Task<AuthLevelDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);

    Task<CompanyOwnerResponseDto> CreateCompanyOwnerAsync(CompanyOwnerDto companyOwnerDto,
        CancellationToken cancellationToken);


}