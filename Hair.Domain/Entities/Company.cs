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
    
    
    public IList<Barber> Barbers { get; private set; } = new List<Barber>();

    public Guid OwnerId { get; set; }
    
    public ApplicationUser Owner { get; set; }
    
    public Company AddImage(IList<string?> imageUrl)
    {
        ImageUrl = imageUrl;
        return this;
    }
}