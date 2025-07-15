using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hair.Infrastructure.Domain.ApplicationUserCompanyConfiguration;

public class ApplicationUserCompanyConfiguration : IEntityTypeConfiguration<ApplicationUserCompany>
{
    public void Configure(EntityTypeBuilder<ApplicationUserCompany> builder)
    {
        
        builder.ToTable("ApplicationUserCompany");
        builder.HasKey(uc => new { uc.ApplicationUserId, uc.CompanyId });

        // Relacija ka ApplicationUser
        builder.HasOne(uc => uc.ApplicationUser)
            .WithMany(u => u.OwnedCompanies)
            .HasForeignKey(uc => uc.ApplicationUserId);

        // Relacija ka Company
        builder.HasOne(uc => uc.Company)
            .WithMany(c => c.ApplicationUserCompanies)
            .HasForeignKey(uc => uc.CompanyId);
        
    }
}