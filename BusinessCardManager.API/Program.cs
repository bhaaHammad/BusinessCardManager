using BusinessCardManager.API.Middleware;
using BusinessCardManager.Application.Interfaces;
using BusinessCardManager.Application.Services;
using BusinessCardManager.Infrastructure;
using BusinessCardManager.Infrastructure.Repositories;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration);


//TODO: refactor
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBusinessCardService, BusinessCardService>();

builder.Services.AddAutoMapper(typeof(BusinessCardManager.Application.Mapping.BusinessCardProfile));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseErrorHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
