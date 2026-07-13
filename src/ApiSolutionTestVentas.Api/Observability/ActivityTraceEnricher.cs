using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace ApiSolutionTestVentas.Api.Observability;

/// <summary>
/// Enricher de Serilog que añade <c>trace_id</c> y <c>span_id</c> (tomados de la
/// <see cref="Activity"/> actual de OpenTelemetry) a cada evento de log.
///
/// Así los logs en JSON que emite el API por stdout —y que recoge Promtail hacia Loki—
/// pueden correlacionarse con su traza en Jaeger desde Grafana (el datasource de Loki
/// detecta el campo <c>trace_id</c> y ofrece "Ver traza").
/// </summary>
public class ActivityTraceEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var activity = Activity.Current;
        if (activity is null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("trace_id", activity.TraceId.ToString()));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("span_id", activity.SpanId.ToString()));
    }
}
