# HeapCare Observability

Shared .NET observability primitives used by HeapCare hospital-system services.

## Packages

| Package | Purpose |
|---|---|
| `HeapCare.Observability` | Structured logging, OTLP tracing and metrics, Prometheus scraping export, and dependency health-check registration |
| `HeapCare.Observability.AspNetCore` | ASP.NET Core host-builder integration for the shared Serilog configuration |

Both packages target .NET 9. They are consumed by the .NET 10 HeapCare API and are built and tested in this repository's CI before publication to GitHub Packages.

## Install

Add the GitHub Packages NuGet feed for the HeapCare organization and authenticate with an identity that has `read:packages` access:

```bash
dotnet add package HeapCare.Observability
dotnet add package HeapCare.Observability.AspNetCore
```

Do not place a package token in project files, build arguments, source control or application logs. Use the package-consumer's secret-management mechanism.

## ASP.NET Core setup

The API uses the ASP.NET Core host integration, OpenTelemetry registration and health-check registration:

```csharp
using HeapCare.Observability.AspNetCore.Extensions;
using HeapCare.Observability.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseHeapCareObservability();
builder.Services.AddObservability(builder.Configuration);
builder.Services.AddObservabilityHealthChecks();

var app = builder.Build();

app.MapPrometheusScrapingEndpoint();
app.Run();
```

A service can register its own additional instrumentations alongside the shared registration. Keep service-specific instrumentation at the application boundary rather than duplicating the common exporters and resource configuration.

## Configuration

`AddObservability` reads:

```json
{
  "ApplicationName": "HeapCare.Api",
  "OpenTelemetry": {
    "ServiceName": "HeapCare.Api",
    "OtlpEndpoint": "http://otel-collector:4317"
  },
  "ElasticConfiguration": {
    "Uri": "http://elasticsearch:9200"
  }
}
```

- `OpenTelemetry:ServiceName` sets the telemetry service resource name. It defaults to `DefaultService` when omitted.
- `OpenTelemetry:OtlpEndpoint` is the OTLP gRPC endpoint. It defaults to `http://localhost:4317`.
- `ApplicationName` is used as the Serilog Application property; otherwise the host environment's application name is used.
- `ElasticConfiguration:Uri` enables the optional Elasticsearch sink. When absent, logging is written to console.

The service should supply environment-specific endpoint values through deployment configuration. The sample values above are for local/container networking only; they are not production addresses.

## Health checks

`AddObservabilityHealthChecks` always registers a basic `self` liveness check and lets the host register additional dependency checks through `HealthCheckDependenciesCollection`. Map readiness/liveness routes in the host using the host's health-check endpoint policy. Do not expose detailed dependency errors or internal topology on a public endpoint.

## Metrics, traces and logs

- OTLP gRPC exports traces and metrics to the configured collector.
- ASP.NET Core and HttpClient instrumentation record request activity.
- SQL Client instrumentation records database spans; the API adds its provider-specific Npgsql instrumentation at the API boundary.
- Runtime and Process instrumentation contribute runtime/process metrics.
- Prometheus scraping export is enabled for the API's metrics endpoint.
- Serilog enriches console/optional Elasticsearch events with service, environment, machine and log-context fields.

Use low-cardinality metric dimensions. Do not log patient names, MRNs, clinical notes, contact information, access/refresh tokens, credentials, OTP values or other patient-identifiable data. Keep trace baggage and exception payloads free of protected information.

## Upstream prerelease dependencies

The current metrics implementation uses `OpenTelemetry.Exporter.Prometheus.AspNetCore` and `OpenTelemetry.Instrumentation.Process`, which are prerelease upstream components. They remain explicitly pinned because the shared API currently depends on their exporter/instrumentation behavior. CI emits NuGet `NU5104` warnings for stable packages that include prerelease dependencies. These warnings are known and must not be hidden or represented as a fully stable upstream dependency set. Revisit the package boundary when stable upstream equivalents are available or when the metrics implementation is deliberately replaced.

The remaining OpenTelemetry core/exporter/hosting/ASP.NET Core/HTTP/runtime/SQL Client references are pinned to stable versions in the 1.15.x family. Review the upstream package release notes and target-framework compatibility before changing this family.

## Build and verification

From the repository root:

```bash
dotnet restore
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet format --verify-no-changes
dotnet pack --configuration Release
```

CI also publishes packages from `main` to GitHub Packages after build, test and format checks pass. A successful package build does not by itself prove every consuming application has restored and exercised the published package; the HeapCare.Local nine-service workflow performs the integrated API/observability verification.
