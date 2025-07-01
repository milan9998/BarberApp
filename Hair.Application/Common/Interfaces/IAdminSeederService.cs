using Microsoft.AspNetCore.Identity;

namespace Hair.Application.Common.Interfaces;

public interface IAdminSeederService
{
    Task SeedAdminAsync();
    
}