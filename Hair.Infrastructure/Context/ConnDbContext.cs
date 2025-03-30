﻿using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Hair.Infrastructure.Context;

public class ConnDbContext : DbContext,IHairDbContext
{
    private readonly string _connectionString;
    
    public ConnDbContext(DbContextOptions<ConnDbContext> options) : base(options) { }
    /*
    public ConnDbContext(DbContextOptions<ConnDbContext> options,IOptions<PostgresDbConfiguration> postgresConfig): base(options)
    {
        _connectionString = postgresConfig.Value.ConnectionString;
    }
*/
    public ConnDbContext()
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    } 
   //    => optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=123");


    public DbSet<Company> Companies { get; set; }
    public DbSet<Barber> Barbers { get; set; }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Customer> Customers { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }
}