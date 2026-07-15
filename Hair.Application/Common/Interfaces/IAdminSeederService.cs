namespace Hair.Application.Common.Interfaces;

public interface IAdminSeederService
{
    Task SeedAdminAsync();
    Task SeedDemoOwnerAsync();
    Task SeedDemoCrmAsync();
}
