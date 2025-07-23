
using Hair.Application.Common.Dto.Barber;
using Hair.Application.Common.Dto.Company;
using Hair.Application.Common.Interfaces;
using Hair.Application.Common.Mappers;
using Hair.Application.Companies.Queries;
using Hair.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hair.Infrastructure.Services;

public class CompanyService(IHairDbContext dbContext,IWebHostEnvironment hostEnvironment,ILogger<CompanyService> _logger) : ICompanyService
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
     public async Task<string> UpdateCompanyAsync(UpdateCompanyDto updateCompanyDto, CancellationToken cancellationToken)
    {
        try
        {
            var companyToUpdate = await dbContext.Companies.Where(c => c.Id == updateCompanyDto.CompanyId)
                .FirstOrDefaultAsync(cancellationToken);
            if (companyToUpdate is null)
            {
                throw new Exception($"Company {updateCompanyDto.CompanyName} does not exist");
            }
            companyToUpdate.UpdateCompanyName(updateCompanyDto.CompanyName);
            var currentImageUrls = companyToUpdate.ImageUrl.ToList();
            //var keptImages = currentImageUrls.Where(url => !updateCompanyDto.ImagesToDelete.Contains(url)).ToList();
            if (updateCompanyDto.ImagesToDelete != null && updateCompanyDto.ImagesToDelete.Any())
            {
                foreach (var image in updateCompanyDto.ImagesToDelete)
                {
                    if (string.IsNullOrWhiteSpace(image)) continue;
                    var relativePath = image.Replace("http://localhost:5045/", "");
                    if (relativePath.StartsWith("images/companies/", StringComparison.OrdinalIgnoreCase))
                    {
                        var filePath = Path.Combine(hostEnvironment.WebRootPath, relativePath);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            currentImageUrls.Remove(image);
                        }
                        else
                        {
                            Console.WriteLine($"WARNING: Image file {filePath} did not exist for deletion for company {updateCompanyDto.CompanyId}.");
                        }
                    }
                    else
                    {
                        throw new Exception($"Greška pri brisanju fajla {image}: Pogrešna putanja slike za kompaniju sa ID-jem: {updateCompanyDto.CompanyId}");
                    }
                }
            }

            if (updateCompanyDto.NewImages != null && updateCompanyDto.NewImages.Any())
            {
                var newUploadedImages = await UploadImageAsync(updateCompanyDto.NewImages);
                foreach (var newUrl in newUploadedImages)
                {
                    if (!string.IsNullOrWhiteSpace(newUrl) && !currentImageUrls.Contains(newUrl))
                    {
                        currentImageUrls.Add(newUrl);
                    }
                }
                //keptImages.AddRange(newUploadedImages.Where(url => !string.IsNullOrWhiteSpace(url)));
            }
            //IList<string?> newImageUrls = null;
            //newImageUrls = await UploadImageAsync(updateCompanyDto.NewImages);
            companyToUpdate.AddImage(currentImageUrls);
            if (!companyToUpdate.ImageUrl.Any())
            {
                throw new Exception("Kompanija mora imati barem jednu sliku.");
            }
            await dbContext.SaveChangesAsync(cancellationToken);
            return "Uspešno izmenjeni podaci kompanije.";
        }
        catch (Exception e)
        {
            throw new Exception($"Error updating company {updateCompanyDto.CompanyName} ", e);
        }
    }
    public async Task<string> DeleteCompanyByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            var companyToDelete = await dbContext.Companies.Where(c => c.Id == companyId).FirstOrDefaultAsync(cancellationToken);
        
            if (companyToDelete == null)
            {
                throw new Exception($"Company {companyId} does not exist");
            }

            if (companyToDelete.ImageUrl != null && companyToDelete.ImageUrl.Any())
            {
                foreach (var image in companyToDelete.ImageUrl)
                {
                    if (string.IsNullOrWhiteSpace(image)) continue;

                    var relativePath = image.Replace("http://localhost:5045/", "");

                    if (relativePath.StartsWith("images/companies/", StringComparison.OrdinalIgnoreCase))
                    {
                        var filePath = Path.Combine(hostEnvironment.WebRootPath, relativePath);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        else
                        {
                            
                            throw new Exception($"Image file {filePath} does not exist");
                        }
                    }
                    else
                    {
                        throw new Exception($"Error deleting file {image} for company with id: {companyId}");
                    }
                }
            }
            
            dbContext.Companies.Remove(companyToDelete);
            await dbContext.SaveChangesAsync(cancellationToken);
            return $"Successfully deleted company with {companyId}";
        }
        catch (Exception e)
        {
            _logger.LogError("Greska koja se dogodila u Company Servisu");
            throw new Exception($"Failed to delete company {companyId}", e);
        }
        
    }
}
