using HeapCare.Observability.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HeapCare.Observability.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddObservabilityHealthChecks(
            this IServiceCollection services,
            Action<HealthCheckDependenciesCollection>? configure = null)
        {
            var deps = new HealthCheckDependenciesCollection();

            // Always add a basic "self" check (liveness)
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("Service is running"));

            // Allow caller to add custom checks
            configure?.Invoke(deps);

            // Register each dependency provided
            foreach (var (name, check) in deps.GetAll())
            {
                services.AddHealthChecks().AddCheck(name, check);
            }

            return services;
        }
    }
}
