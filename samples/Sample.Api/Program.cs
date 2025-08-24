using CompileCare.Observability.AspNetCore.Extensions;
using CompileCare.Observability.Extensions;
using Sample.Api.Extensions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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
//app.MapPrometheusScrapingEndpoint();
app.Run();
