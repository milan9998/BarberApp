using Microsoft.AspNetCore.Http;

namespace Hair.Application.Common.Dto.Company;

public record UpdateCompanyDto(
    Guid CompanyId,
    string CompanyName,
    IList<IFormFile>? NewImages,
    IList<string>? ImagesToDelete
);