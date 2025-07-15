using System.ComponentModel.DataAnnotations.Schema;

namespace Hair.Domain.Entities;

public class ApplicationUserCompany
{
    public Guid Id { get; set; }
    
    public string  ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser ApplicationUser { get; set; }

    
    public Guid CompanyId { get; set; }
    [ForeignKey("CompanyId")]
    public Company Company { get; set; }
    
    
}