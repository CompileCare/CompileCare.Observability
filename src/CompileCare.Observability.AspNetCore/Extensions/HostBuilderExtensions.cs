using HeapCare.Observability.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace HeapCare.Observability.AspNetCore.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseHeapCareObservability(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, loggerConfig) =>
            {
                LoggerConfigurator.ConfigureLogger(context, loggerConfig);
            });
        }
    }
}
