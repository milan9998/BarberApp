using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hair.Infrastructure.Domain.ApplicationUserConfiguration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(u => u.Company)
               .WithOne(c => c.Owner)
               .HasForeignKey<Company>(c => c.OwnerId)
               .IsRequired(false);
    }
}