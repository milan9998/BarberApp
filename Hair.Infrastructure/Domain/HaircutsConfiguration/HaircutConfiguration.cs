using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hair.Infrastructure.Domain.HaircutsConfiguration;

public class HaircutConfiguration : IEntityTypeConfiguration<Haircut>
{
    public void Configure(EntityTypeBuilder<Haircut> builder)
    {
        builder.ToTable("Haircuts");
        builder.Property(c => c.Id).ValueGeneratedNever();
        
        builder.HasOne(h => h.Company)
            .WithMany(c => c.Haircuts)  
            .HasForeignKey(h => h.CompanyId) 
            .OnDelete(DeleteBehavior.Cascade);
    }
}
