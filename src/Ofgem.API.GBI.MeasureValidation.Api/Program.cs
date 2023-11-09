using Ofgem.API.GBI.MeasureValidation.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureApplicationServices(builder.Configuration);

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddLogsConfiguration(builder.Configuration);
}

builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.MapMeasureValidationEndpoints();
app.MapGetErrorsEndpoints();

app.Run();

public partial class Program { }
