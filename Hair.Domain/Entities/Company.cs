﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Hair.Domain.Entities;

public class Company
{
    public Company( string companyName)
    {
        CompanyName = companyName;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; private set; }
    public string CompanyName { get; private set; }
    
    public IList<string?> ImageUrl { get; private set; }
    
    public ICollection<ApplicationUserCompany> ApplicationUserCompanies { get; set; } = new List<ApplicationUserCompany>();
    public IList<Barber> Barbers { get; private set; } = new List<Barber>();
    
    public IList<Haircut> Haircuts { get; private set; } = new List<Haircut>();
    
    public Company AddImage(IList<string?> imageUrl)
    {
        ImageUrl = imageUrl;
        return this;
    }
    public Company UpdateCompanyName(string companyName)
    {
        CompanyName = companyName;
        return this;
    }
    
}