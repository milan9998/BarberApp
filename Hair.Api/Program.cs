using System.ComponentModel;
using FluentValidation.AspNetCore;
using Hair.Api.Filters;
using Hair.Application;
using Hair.Application.Common.Interfaces;
using Hair.Domain.Entities;
using Hair.Infrastructure;
using Hair.Infrastructure.Configuration;
using Hair.Infrastructure.Context;
using Hair.Infrastructure.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() // minimalni nivo logovanja (može i Information ili Warning)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // manje detalja za Microsoft namespace
    .Enrich.FromLogContext()
    .WriteTo.Console() // log u konzolu
    .WriteTo.File(
        "Logs/logs-.txt",               // folder Logs i fajl sa dnevnim rotiranjem
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

// POVEZI SERILOG SA HOST BUILDEROM
builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://127.0.0.1:4200",
                "https://barbercontrolhq.com",
                "https://www.barbercontrolhq.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ApiExceptionFilterAttribute>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiExceptionFilterAttribute>();  
});
//builder.Services.AddControllers();
//options => options.Filters.Add<ApiExceptionFilterAttribute>()
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddScoped<IHairDbContext, ConnDbContext>();
builder.Services.AddDbContext<ConnDbContext>(options =>     
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddScoped<ApiExceptionFilterAttribute>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ConnDbContext>()
    .AddDefaultTokenProviders();


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await AdminSeederService.SeedRolesAsync(roleManager);
    var adminSeeder = scope.ServiceProvider.GetRequiredService<IAdminSeederService>();
    await adminSeeder.SeedAdminAsync();
    await adminSeeder.SeedDemoOwnerAsync();
    await adminSeeder.SeedDemoCrmAsync();
}

/*app.UseExceptionHandler(errorApp =>
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
});*/

app.UseDefaultFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Avoid Cloudflare/browser serving a stale SPA while iterating on branding.
        ctx.Context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
        ctx.Context.Response.Headers.Pragma = "no-cache";
        ctx.Context.Response.Headers.Expires = "0";
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cloudflare tunnel hits the API over plain HTTP on localhost — skip HTTPS redirect in Development.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");
app.MapControllers();

// Serve Angular SPA for non-API routes (Cloudflare front can use same port as API).
app.MapFallbackToFile("index.html");

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