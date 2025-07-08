using Hair.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hair.Application.Common.Interfaces;

public interface IHairDbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Barber> Barbers { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<AnonymousUser> AnonymousUsers { get; set; }
    public DbSet<Haircut> Haircuts { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    
}