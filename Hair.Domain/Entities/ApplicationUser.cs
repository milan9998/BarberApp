using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hair.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public Role Role { get; set; }
    
   // public Guid? CompanyId { get; set; }
    
   public ICollection<ApplicationUserCompany> OwnedCompanies { get; set; }
   
    public virtual Barber Barber { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
}