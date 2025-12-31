using CompileCare.Observability.AspNetCore.Extensions;
using CompileCare.Observability.Extensions;
using Sample.Api.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog Logging - this is optional as we have extended the HostBuilder to use Serilog
// builder.AddCompileCareLogging();

// Plug in Observability (Serilog + Elastic)
builder.Host.UseCompileCareObservability();

// Otel 
builder.Services.AddObservability(builder.Configuration);

// Health Checks
builder.Services.AddObservabilityHealthChecks(deps =>
{
    //// Example: SQL Server dependency
    //deps.Add("sql", new SqlConnectionHealthCheck(builder.Configuration.GetConnectionString("DefaultConnection")));

    //// Example: custom dependency check
    //deps.Add("external-api", new ExternalApiHealthCheck("https://example.com/health"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());
var app = builder.Build();
app.MapPrometheusScrapingEndpoint(); // /metrics
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample API v1");
        options.RoutePrefix = string.Empty; // makes Swagger UI available at `/`
    });
}

app.MapGet("/", () => "Hello World!");
app.MapEndpoints();
app.MapHealthEndpoints();
app.Run();
