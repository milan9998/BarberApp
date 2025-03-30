using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Dto.Company;

namespace Hair.Application.Common.Interfaces;

public interface ICompanyService
{
    Task<CompanyCreateDto> CreateCompanyAsync(
        CompanyCreateDto scheduleAppointmentCreateDto, 
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