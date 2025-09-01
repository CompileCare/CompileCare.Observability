using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Sample.Api.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static void AddObservability(this WebApplicationBuilder builder)
        {
            var otelConfig = builder.Configuration.GetSection("OpenTelemetry");
            var serviceName = otelConfig["ServiceName"] ?? "DefaultService";
            var otlpEndpoint = otelConfig["OtlpEndpoint"] ?? "http://localhost:4317";
            var resourceBuilder = ResourceBuilder.CreateDefault().AddService(serviceName);

            // Traces + Metrics
            builder.Services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName))
                .WithTracing(traces =>
                {
                    traces.SetResourceBuilder(resourceBuilder)
                     .AddAspNetCoreInstrumentation()
                     .AddHttpClientInstrumentation()
                     .AddSqlClientInstrumentation()
                     .AddOtlpExporter(o =>
                     {
                         o.Endpoint = new Uri(otlpEndpoint); // collector gRPC endpoint
                     });
                })
                .WithMetrics(metrics =>
                {
                    metrics.SetResourceBuilder(resourceBuilder)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri(otlpEndpoint);
                            o.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });

                    // Register Prometheus exporter separately
                    metrics.AddPrometheusExporter();
                });

            // Logs
            builder.Logging.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder)
                 .AddOtlpExporter(opt =>
                 {
                     opt.Endpoint = new Uri(otlpEndpoint);
                     opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                 });
            });
        }
    }
}
