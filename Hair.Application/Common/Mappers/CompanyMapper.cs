using Hair.Application.Common.Dto.Company;
using Hair.Domain.Entities;

namespace Hair.Application.Common.Mappers;

public static partial class CompanyMapper
{
    public static Company FromCreateDtoToEntity(this CompanyCreateDto dto)
    {
        var company = new Company(dto.CompanyName);
        company.AddImage(dto.ImageUrl);
        return company;
    }
}