using FluentResults.Extensions.AspNetCore;
using SmartCatalog.Api.FluentResults;
using SmartCatalog.BusinessLogic.Extensions;
using SmartCatalog.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBusinessLogic();
builder.Services.AddDataAccess(builder.Configuration);

AspNetCoreResult.Setup(config => config.DefaultProfile = new AspNetCoreResultEndpointProfile());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();