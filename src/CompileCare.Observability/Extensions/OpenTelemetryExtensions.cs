using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
namespace CompileCare.Observability.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
        {
            var otelConfig = configuration.GetSection("OpenTelemetry");
            var serviceName = otelConfig["ServiceName"] ?? "DefaultService";
            var otlpEndpoint = otelConfig["OtlpEndpoint"] ?? "http://localhost:4317";
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);
            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource
                    .AddService(serviceName)
                    .AddTelemetrySdk()
                    .AddEnvironmentVariableDetector())
                .WithTracing(tracing =>
                {
                    tracing.AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddSqlClientInstrumentation()
                           .AddOtlpExporter(o =>
                           {
                               o.Endpoint = new Uri(otlpEndpoint); // OTLP gRPC
                           }); // default exporter (OTLP)
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                           .AddHttpClientInstrumentation()
                           .AddRuntimeInstrumentation()
                           .AddProcessInstrumentation()
                           .AddOtlpExporter(o =>
                           {
                               o.Endpoint = new Uri(otlpEndpoint); // OTLP gRPC
                           });
                });

            return services;
        }
    }
}
