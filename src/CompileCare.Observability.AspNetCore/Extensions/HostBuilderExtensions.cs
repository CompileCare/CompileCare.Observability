using CompileCare.Observability.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CompileCare.Observability.AspNetCore.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseCompileCareObservability(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, loggerConfig) =>
            {
                LoggerConfigurator.ConfigureLogger(context, loggerConfig);
            });
        }
    }
}
