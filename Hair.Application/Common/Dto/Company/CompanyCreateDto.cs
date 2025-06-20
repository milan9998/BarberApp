using Microsoft.AspNetCore.Http;

namespace Hair.Application.Common.Dto.Company;

public record CompanyCreateDto(string CompanyName,IList<string?> Image)
{
    
}