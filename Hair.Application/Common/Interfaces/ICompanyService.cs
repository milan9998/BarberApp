using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Dto.Company;
using Microsoft.AspNetCore.Http;

namespace Hair.Application.Common.Interfaces;

public interface ICompanyService
{
    Task<CompanyCreateDto> CreateCompanyAsync(
        string companyName, IFormFile? image, 
        CancellationToken cancellationToken
    );


    Task<List<BarberFullDetailsDto>> CompanyDetailsByIdAsync(
        Guid companyId,
        CancellationToken cancellationToken
    );

    Task<List<CompanyDetailsDto>> GetAllCompaniesAsync(
        CompanyDetailsDto companyDetailsDto,
        CancellationToken cancellationToken
    );
    




}