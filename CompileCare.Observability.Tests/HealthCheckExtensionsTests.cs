using FluentAssertions;
using CompileCare.Observability.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CompileCare.Observability.Tests
{
    public class HealthCheckExtensionsTests
    {
        [Fact]
        public async Task AddObservabilityHealthChecks_Should_Register_Self_Check()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();

            // Act
            services.AddObservabilityHealthChecks();
            var provider = services.BuildServiceProvider();
            var healthChecks = provider.GetRequiredService<HealthCheckService>();

            // Assert
            healthChecks.Should().NotBeNull();

            var report = await healthChecks.CheckHealthAsync();

            report.Entries.Should().ContainKey("self");
            report.Entries["self"].Status.Should().Be(HealthStatus.Healthy);
        }
    }
}
