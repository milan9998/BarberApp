﻿using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hair.Infrastructure.Domain.BarberConfiguration;

public class BarberConfiguration : IEntityTypeConfiguration<Barber>
{
    public void Configure(EntityTypeBuilder<Barber> builder)
    {
        builder.ToTable("Barbers");
        builder.Property(b => b.BarberId).ValueGeneratedNever();
        builder.HasKey(b => b.BarberId);


        builder.HasOne(b => b.Company)
            .WithMany(c => c.Barbers);
        /*
        builder.HasOne(b => b.ApplicationUser)
            .WithOne(c=>c.Barber)
            .HasForeignKey(b => b.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);
*/

    }
}