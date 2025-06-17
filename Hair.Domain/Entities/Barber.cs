using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hair.Domain.Entities;

public class Barber : IdentityUser<Guid>
{
    public Barber(string barberName, string phoneNumber, string email, TimeSpan? individualStartTime, TimeSpan? individualEndTime)
    {
        
        BarberName = barberName;
        PhoneNumber = phoneNumber;
        Email = email;
        IndividualStartTime = individualStartTime;
        IndividualEndTime = individualEndTime;
        UserName = email;
        
        Id = Guid.NewGuid();
        //BarberId = Guid.NewGuid();
    }

    [NotMapped] public Guid BarberId => Id;
    public string BarberName { get; private set; }
    public string PhoneNumber { get; private set; }
    
    public Role Role { get; set; }
    
  //  public string PictureUrl { get; private set; }//****** dodati
    public string Email { get; private set; }
    
    
    
    public Company Company { get; private set; }
   
    public TimeSpan? IndividualStartTime { get; private set; } 
    public TimeSpan? IndividualEndTime { get; private set; }   
    
    
    
    public Barber AddBarberCompany(Company company)
    {
        Company = company;
        
        return this;
    }


}