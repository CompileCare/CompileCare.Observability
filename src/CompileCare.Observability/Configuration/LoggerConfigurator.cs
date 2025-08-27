using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers;
using Serilog.Sinks.Elasticsearch;


namespace CompileCare.Observability.Configuration
{
    public static class LoggerConfigurator
    {
        public static void ConfigureLogger(HostBuilderContext context, LoggerConfiguration loggerConfig)
        {
            var configuration = context.Configuration;
            var environment = context.HostingEnvironment;

            var elasticUri = configuration["ElasticConfiguration:Uri"];

            loggerConfig
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Application", configuration["ApplicationName"] ?? environment.ApplicationName)
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .WriteTo.Console();

            if (!string.IsNullOrEmpty(elasticUri))
            {
                loggerConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"compilecare-logs-{DateTime.UtcNow:yyyy.MM}"
                });
            }
        }
    }
}
