using Microsoft.AspNetCore.Http;

namespace Hair.Application.Common.Dto.Company;

public record CompanyCreateRequestDto(string CompanyName,IFormFile? Image);