using System.ComponentModel;
using FluentValidation.AspNetCore;
using Hair.Api.Filters;
using Hair.Application;
using Hair.Infrastructure;
using Hair.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
//options => options.Filters.Add<ApiExceptionFilterAttribute>()
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();
builder.Services.Configure<PostgresDbConfiguration>(
    builder.Configuration.GetSection("PostgresDbConfiguration"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
