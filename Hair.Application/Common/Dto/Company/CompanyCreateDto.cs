﻿using Microsoft.AspNetCore.Http;

namespace Hair.Application.Common.Dto.Company;

public record CompanyCreateDto(string CompanyName,string? Image)
{
    
}