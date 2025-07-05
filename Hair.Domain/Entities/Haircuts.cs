namespace Hair.Domain.Entities;

public class Haircuts
{
    public Haircuts(int duration, string haircutType)
    {
     
        Duration = duration;
        HaircutType = haircutType;
    }

    public Guid Id { get; private set; }
    public int Duration { get; private set; }
    public Guid CompanyId { get; private set; }
    public string HaircutType { get; private set; }
    public decimal Price { get; private set; }
    public Company Company { get; set; }
    
   
    
}