using System.ComponentModel;
using FluentValidation.AspNetCore;
using Hair.Api.Filters;
using Hair.Application;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Infrastructure;
using Hair.Infrastructure.Configuration;
using Hair.Infrastructure.Context;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
//options => options.Filters.Add<ApiExceptionFilterAttribute>()
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddScoped<IHairDbContext, ConnDbContext>();
builder.Services.AddDbContext<ConnDbContext>(options =>     
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ConnDbContext>()
    .AddDefaultTokenProviders();

/*builder.Services.AddIdentity<Barber, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
    })
    .AddEntityFrameworkStores<ConnDbContext>()
    .AddDefaultTokenProviders();*/

/*
builder.Services.Configure<PostgresDbConfiguration>(
    builder.Configuration.GetSection("PostgresDbConfiguration"));
*/

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var adminSeeder = scope.ServiceProvider.GetRequiredService<IAdminSeederService>();
    await adminSeeder.SeedAdminAsync();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            var err = new { message = error.Error.Message };
            await context.Response.WriteAsJsonAsync(err);
        }
    });
});

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();
app.UseCors("AllowFrontend");



app.Run();
/*
 
  "PostgresDbConfiguration": {
    "DbHost": "localhost",
    "DbPort": "5432",
    "DbName": "postgres",
    "UserName": "postgres",
    "Password": "123"
  }
 */