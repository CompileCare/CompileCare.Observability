using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace HeapCare.Observability.Extensions
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddHeapCareLogging(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", configuration["ApplicationName"] ?? environment.ApplicationName)
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            // Apply Elasticsearch sink if configured
            var elasticUri = configuration["ElasticConfiguration:Uri"];
            if (!string.IsNullOrEmpty(elasticUri))
            {
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithMachineName()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                    {
                        AutoRegisterTemplate = true,
                        IndexFormat = $"heapcare-logs-{DateTime.UtcNow:yyyy.MM}"
                    })
                    .CreateLogger();
            }

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            return services;
        }
    }
}
