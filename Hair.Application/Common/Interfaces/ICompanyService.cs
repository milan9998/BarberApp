using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Dto.Company;
using Microsoft.AspNetCore.Http;

namespace Hair.Application.Common.Interfaces;

public interface ICompanyService
{
    Task<CompanyCreateDto> CreateCompanyAsync(
        string companyName, IList<IFormFile?> images, 
        CancellationToken cancellationToken
    );

    Task<string> UpdateCompanyAsync(UpdateCompanyDto updateCompanyDto, CancellationToken cancellationToken);

    Task<string> DeleteCompanyByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken);

    Task<List<BarberFullDetailsDto>> CompanyDetailsByIdAsync(
        Guid companyId,
        CancellationToken cancellationToken
    );

    Task<List<CompanyDetailsDto>> GetAllCompaniesAsync(
        CompanyDetailsDto companyDetailsDto,
        CancellationToken cancellationToken
    );

    Task<CompanyDetailsDto>
        GetCompanyDetailsById
        (Guid CompanyId, CancellationToken cancellationToken);





}