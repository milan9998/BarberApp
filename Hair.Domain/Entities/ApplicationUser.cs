﻿using Hair.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Hair.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public Role Role { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public IList<Company> Companies { get; set; } = new List<Company>();
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
}