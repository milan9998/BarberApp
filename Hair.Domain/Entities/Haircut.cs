namespace Hair.Domain.Entities;

public class Haircut
{
    public Haircut(int duration, string haircutType,decimal price)
    {
     
        Duration = duration;
        HaircutType = haircutType;
        Id = Guid.NewGuid();
        Price = price;
    }

    public Guid Id { get; private set; }
    public int Duration { get; private set; }
    public Guid CompanyId { get; private set; }
    public string HaircutType { get; private set; }
    
    public decimal Price { get; private set; }
    public Company Company { get; set; }

    public Haircut AddCompanyCompany(Company company)
    {
        Company = company;
        return this;
    }
    public Haircut UpdateHaircutType(string haircutType)
    {
        HaircutType = haircutType;
        return this;
    }

    public Haircut UpdatePrice(decimal price)
    {
        Price = price;
        return this;
    }

    public Haircut UpdateDuration(int duration)
    {
        Duration = duration;
        return this;
    }
   
    
}