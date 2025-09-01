using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CompileCare.Observability.HealthChecks
{
    public class HealthCheckDependenciesCollection
    {
        private readonly List<(string name, IHealthCheck check)> _dependencies
            = new List<(string, IHealthCheck)>();

        public void Add(string name, IHealthCheck check)
        {
            _dependencies.Add((name, check));
        }

        public IEnumerable<(string name, IHealthCheck check)> GetAll()
        {
            return _dependencies;
        }
    }
}
