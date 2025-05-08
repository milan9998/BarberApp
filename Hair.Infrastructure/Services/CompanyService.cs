using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Application.Common.Mappers;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hair.Infrastructure.Services;

public class CompanyService (IHairDbContext dbContext) : ICompanyService
{

    public async Task<string> UploadImageToDrive([FromForm]IFormFile image)
    {
        
        var credential = GoogleCredential.FromFile("credentials.json")
            .CreateScoped(DriveService.Scope.DriveFile);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "YourAppName",
        });

        // Priprema fajla za upload
        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = image.FileName,
            MimeType = image.ContentType
        };

        using var stream = image.OpenReadStream();

        // Upload na Google Drive
        var request = service.Files.Create(fileMetadata, stream, image.ContentType);
        request.Fields = "id, webViewLink";
        var result = await request.UploadAsync();

        if (result.Status == UploadStatus.Completed)
        {
            var file = request.ResponseBody;
            return ("Successfully uploaded a new image");

        }
        else
        {
            return ("Failed to upload a new image");
        }
    }
    
    
    
    public async Task<CompanyCreateDto> CreateCompanyAsync(CompanyCreateDto companyCreate,
        CancellationToken cancellationToken)
    {
        var x = await dbContext.Companies.Where(c => c.CompanyName == companyCreate.CompanyName)
            .FirstOrDefaultAsync();
        if (x is  not null)
        {
            throw new Exception($"Company {companyCreate.CompanyName} already exists");
        }
        
     
        Company company = new Company(companyCreate.CompanyName);
        
        
        var companySaved = companyCreate.FromCreateDtoToEntity();
        dbContext.Companies.Add(companySaved);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return new CompanyCreateDto(company.CompanyName,company.ImageUrl);
    }

    public async Task<List<BarberFullDetailsDto>> CompanyDetailsByIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var barbers = await dbContext.Barbers.Include(x => x.Company)
            .Where(x => x.Company.Id == companyId)
            .ToListAsync(cancellationToken);


        if (barbers == null || barbers.Count == 0)
        {
            return new List<BarberFullDetailsDto>();
        }


        return barbers.Select(barber =>
            new BarberFullDetailsDto(barber.BarberId,barber.BarberName, barber.Company.CompanyName)).ToList();
    }

    public async Task<List<CompanyDetailsDto>> GetAllCompaniesAsync(CompanyDetailsDto companyDetailsDto, CancellationToken cancellationToken)
    {
        var companies = await dbContext.Companies.ToListAsync(cancellationToken);

        var result = companies.Select(x => new CompanyDetailsDto()
        {
            CompanyId = x.Id,
            CompanyName = x.CompanyName,
            ImageUrl = x.ImageUrl,
        }).ToList();
        return result;
    }
}