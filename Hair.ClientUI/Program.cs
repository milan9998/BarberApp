using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Hair.Application;
using Hair.Application.Common.Interfaces;
using Hair.ClientUI.Components;
using Hair.Infrastructure;
using Hair.Infrastructure.Context;
using Hair.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container before Build()
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register your scoped services here before Build()

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5045/") });
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IHairDbContext, ConnDbContext>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddDbContext<ConnDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddBlazorise(options => { options.Immediate = true; })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();
builder.Services.AddApplication();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();