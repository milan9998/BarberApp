using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hair.Domain.Entities;

public class Barber 
{
    public Barber(string barberName, string phoneNumber, string email, TimeSpan individualStartTime, TimeSpan individualEndTime)
    {
        
        BarberName = barberName;
        PhoneNumber = phoneNumber;
        Email = email;
        IndividualStartTime = individualStartTime;
        IndividualEndTime = individualEndTime;
       
        
        BarberId = Guid.NewGuid();
        //BarberId = Guid.NewGuid();
    }

    public string ApplicationUserId { get; private set; }
    public virtual ApplicationUser ApplicationUser { get; private set; }
     public Guid BarberId {get; private set;}
    public string BarberName { get; private set; }
    public string PhoneNumber { get; private set; }
    
    public Role Role { get; set; }
    
  //  public string PictureUrl { get; private set; }//****** dodati
    public string Email { get; private set; }
    
    public Company Company { get; private set; }
   
    public TimeSpan IndividualStartTime { get; private set; } 
    public TimeSpan IndividualEndTime { get; private set; }   
    
    
    
    public Barber AddBarberCompany(Company company)
    {
        Company = company;
        
        return this;
    }

    public void SetApplicationUserId(string userId)
    {
        ApplicationUserId = userId;
    }
    public Barber UpdateBarberName(string newBarberName)
    {
        BarberName = newBarberName;
        return this;
    }
    public Barber UpdateBarberPhoneNumber(string newPhoneNumber)
    {
        PhoneNumber = newPhoneNumber;
        return this;
    }
    public Barber UpdateBarberEmail(string newEmail)
    {
        Email = newEmail;
        return this;
    }
    public Barber UpdateIndividualStartTime(TimeSpan newIndividualStartTime)
    {
        IndividualStartTime = newIndividualStartTime;
        return this;
    }
    public Barber UpdateIndividualEndTime(TimeSpan newIndividualEndTime)
    {
        IndividualEndTime = newIndividualEndTime;
        return this;
    }

    public Barber UnsetCompany()
    {
        typeof(Barber)
            .GetProperty("CompanyId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            ?.SetValue(this, null);
        return this;
    }

}