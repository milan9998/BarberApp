using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hair.Infrastructure.Domain.AppointmentConfiguration;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointment");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.HasOne(a => a.ApplicationUser)           // jedan korisnik
            .WithMany(u => u.Appointments)            // može imati više termina
            .HasForeignKey(a => a.ApplicationUserId)  // FK u Appointment
            .OnDelete(DeleteBehavior.Cascade);
    }
}