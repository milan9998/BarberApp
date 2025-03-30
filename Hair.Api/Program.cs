using System.ComponentModel;
using FluentValidation.AspNetCore;
using Hair.Api.Filters;
using Hair.Application;
using Hair.Application.Common.Interfaces;
using Hair.Infrastructure;
using Hair.Infrastructure.Configuration;
using Hair.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


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
/*
builder.Services.Configure<PostgresDbConfiguration>(
    builder.Configuration.GetSection("PostgresDbConfiguration"));
*/
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

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