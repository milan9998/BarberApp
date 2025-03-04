using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hair.Infrastructure.Domain.BarberConfiguration;

public class BarberConfiguration : IEntityTypeConfiguration<Barber>
{
    public void Configure(EntityTypeBuilder<Barber> builder)
    {
        builder.ToTable("Barbers");
        builder.Property(b => b.BarberId).ValueGeneratedNever();


        builder.HasOne(b => b.Company)
            .WithMany(c => c.Barbers);


    }
}