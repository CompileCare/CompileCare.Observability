using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Sample.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static void AddObservability(this WebApplicationBuilder builder)
    {
        var otelConfig = builder.Configuration.GetSection("OpenTelemetry");
        var serviceName = otelConfig["ServiceName"] ?? "DefaultService";
        var otlpEndpoint = otelConfig["OtlpEndpoint"] ?? "http://localhost:4317";
        var endpoint = new Uri(otlpEndpoint);
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = endpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            })
            .WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = endpoint;
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
                    .AddPrometheusExporter();
            });

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = endpoint;
                    exporter.Protocol = OtlpExportProtocol.Grpc;
                });
        });
    }
}
