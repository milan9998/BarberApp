using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hair.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public Role Role { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public Company Company { get; set; }
}