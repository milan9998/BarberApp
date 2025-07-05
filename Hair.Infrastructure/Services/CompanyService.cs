
using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Application.Common.Mappers;
using Hair.Application.Companies.Queries;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hair.Infrastructure.Services;

public class CompanyService(IHairDbContext dbContext) : ICompanyService
{

    public async Task<List<string>> UploadImageAsync([FromForm] IList<IFormFile> image)
    {
        var urls = new List<string>();

        if (image == null || image.Count == 0)
            throw new ArgumentException("Image file is empty.");

        // folder: wwwroot/images/companies
        var folderName = Path.Combine("wwwroot", "images", "companies");
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        for (int i = 0; i < image.Count; i++)
        {
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image[i].FileName);
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image[i].CopyToAsync(stream);
            }

            var fullUrl = $"http://localhost:5045/images/companies/{uniqueFileName}";
            urls.Add(fullUrl);
        }




        // Return full URL path
        // Change localhost:5000 if necessary
        return urls;
    }

    public async Task<CompanyDetailsDto> GetCompanyDetailsById(Guid CompanyId, CancellationToken cancellationToken)
    {

        var company = await dbContext.Companies.Where(c => c.Id == CompanyId).FirstOrDefaultAsync(cancellationToken);
        var toReturnCompanyDetails = new CompanyDetailsDto(company.Id, company.CompanyName, company.ImageUrl);
        return toReturnCompanyDetails;

    }

    public async Task<CompanyCreateDto> CreateCompanyAsync(string companyName, IList<IFormFile?> images,
        CancellationToken cancellationToken)
    {
        var companyExists = await dbContext.Companies.Where(c => c.CompanyName == companyName)
            .FirstOrDefaultAsync();
        if (companyExists is not null)
        {
            throw new Exception($"Company {companyName} already exists");
        }

        IList<string?> imageUrl = null;

        for (int i = 0; i < images.Count; i++)
        {
            if (images is not null)
            {
                imageUrl = await UploadImageAsync(images);
            }
        }

        Company company = new Company(companyName);
        company.AddImage(imageUrl);
        dbContext.Companies.Add(company);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CompanyCreateDto(company.CompanyName, company.ImageUrl);
    }

    public async Task<List<BarberFullDetailsDto>> CompanyDetailsByIdAsync(Guid companyId,
        CancellationToken cancellationToken)
    {
        var barbers = await dbContext.Barbers.Include(x => x.Company)
            .Where(x => x.Company.Id == companyId)
            .ToListAsync(cancellationToken);


        if (barbers == null || barbers.Count == 0)
        {
            return new List<BarberFullDetailsDto>();
        }


        return barbers.Select(barber =>
            new BarberFullDetailsDto(barber.BarberId, barber.BarberName, barber.Company.CompanyName)).ToList();
    }

    public async Task<List<CompanyDetailsDto>> GetAllCompaniesAsync(CompanyDetailsDto companyDetailsDto,
        CancellationToken cancellationToken)
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
