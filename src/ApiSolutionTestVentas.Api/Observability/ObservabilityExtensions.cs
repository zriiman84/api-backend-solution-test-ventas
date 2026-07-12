using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ApiSolutionTestVentas.Api.Observability;

/// <summary>
/// Configuración centralizada de OpenTelemetry para el API.
///
/// Exporta MÉTRICAS y TRAZAS por OTLP hacia un OpenTelemetry Collector.
/// El endpoint se toma de la variable de entorno OTEL_EXPORTER_OTLP_ENDPOINT
/// (ej. http://otel-collector:4317). Si NO está definida, la instrumentación
/// se registra pero no exporta nada (cero ruido en local sin stack levantado).
///
/// Los LOGS se exportan por separado vía el sink OTLP de Serilog en Program.cs,
/// para reutilizar la configuración de Serilog que ya existía.
/// </summary>
public static class ObservabilityExtensions
{
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var serviceName = builder.Configuration["OTEL_SERVICE_NAME"] ?? "api-solution-test-ventas";
        var serviceVersion = typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "1.0.0";
        var environmentName = builder.Environment.EnvironmentName;
        var exportEnabled = !string.IsNullOrWhiteSpace(otlpEndpoint);

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                .AddAttributes(new KeyValuePair<string, object>[]
                {
                    new("deployment.environment", environmentName)
                }))

            // ---------- MÉTRICAS (RED: tasa, errores, latencia + runtime .NET) ----------
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()   // http.server.request.duration, active_requests...
                    .AddHttpClientInstrumentation()   // llamadas salientes
                    .AddRuntimeInstrumentation();      // GC, heap, threads del runtime .NET

                if (exportEnabled)
                {
                    metrics.AddOtlpExporter();
                }
            })

            // ---------- TRAZAS (request -> controllers -> EF Core/SQL -> HttpClient) ----------
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(options => options.RecordException = true)
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                if (exportEnabled)
                {
                    tracing.AddOtlpExporter();
                }
            });

        return builder;
    }
}
