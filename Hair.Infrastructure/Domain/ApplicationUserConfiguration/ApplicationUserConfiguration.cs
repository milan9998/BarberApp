using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hair.Infrastructure.Domain.ApplicationUserConfiguration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        /*
        builder.HasMany(c => c.Companies)
            .WithOne(u => u.Owner)
            .HasForeignKey(c => c.CompanyOwnerId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
            */
    }
}