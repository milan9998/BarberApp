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
    
    public string? ImageUrl { get; private set; }
    
    
    public IList<Barber> Barbers { get; private set; } = new List<Barber>();

    public Company AddImage(string imageUrl)
    {
        ImageUrl = imageUrl;
        return this;
    }
}