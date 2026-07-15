# 🔭 Observabilidad local — API solution-test-ventas

Stack local de los **3 pilares** (métricas, trazas, logs) para el API .NET, alineado a las
herramientas del **laboratorio DMC (observabilidad-360)**:
**OpenTelemetry + Prometheus + Jaeger + Loki/Promtail + Grafana**, con alertas vía Alertmanager
y métricas de infraestructura del host vía Node Exporter.

```
                             ┌──► Prometheus (métricas) ──► Alertmanager (alertas :9093)
API .NET ─OTLP(gRPC :4317)──► OTel Collector ─┴──► Jaeger      (trazas)
API .NET (stdout JSON) ─────► Promtail ───────────► Loki       (logs)
Node Exporter ──────────────────► Prometheus       (CPU/RAM/disco del host)
                                     todo visualizado en Grafana (:3000)
```

> **Trazas por Jaeger** (no Tempo) y **logs por Promtail** (no OTLP): es el stack tal como se
> enseña en el laboratorio. Las **métricas y trazas** siguen saliendo del API por OTLP hacia el
> Collector; los **logs** los recoge Promtail desde el stdout del contenedor.

## Requisitos
- Docker + Docker Compose (en este equipo: dentro de WSL Ubuntu).
- Si tienes otro stack usando los puertos 9090/3000/3100/16686/9093/9100, detenlo antes para no chocar.

## 1. Levantar el stack de observabilidad
```bash
cd observability
docker compose up -d
docker compose ps          # los 8 servicios deben estar Up
```

| Servicio | URL local | Para qué |
|---|---|---|
| Grafana | http://localhost:3000 (admin/admin) | Ver todo (dashboards, trazas, logs, alertas) |
| Prometheus | http://localhost:9090 | Consultar métricas y evaluar reglas de alerta |
| Alertmanager | http://localhost:9093 | Enrutamiento / agrupación / silenciado de alertas |
| Jaeger | http://localhost:16686 | UI de trazas (también consultable desde Grafana) |
| Loki | http://localhost:3100 | API de logs (se usa desde Grafana) |
| Promtail | (sin UI) | Recoge el stdout de los contenedores → Loki |
| OTel Collector | OTLP :4317 (gRPC) / :4318 (HTTP), métricas en :8889 | Recibe métricas+trazas del API y las reparte |
| Node Exporter | http://localhost:9100/metrics | Métricas de infraestructura del host (CPU/RAM/disco) |

Datasources y el dashboard **Golden Signals** ("API solution-test-ventas - Golden Signals
(RED + Saturation)") quedan provisionados automáticamente en Grafana (carpeta *API solution-test-ventas*).

## 2. Levantar el API conectado a la observabilidad
Desde la raíz del repo del backend (necesitas un `.env` con credenciales de Azure SQL — ver `.env.example`):
```bash
docker compose -f docker-compose.yaml -f docker-compose.observability.yml up -d --build
```
Esto añade al contenedor del API las variables:
- `OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4317`
- `OTEL_EXPORTER_OTLP_PROTOCOL=grpc`
- `OTEL_SERVICE_NAME=api-solution-test-ventas`

y lo une a la red `observability`. El API queda en http://localhost:8080.

> Si NO defines `OTEL_EXPORTER_OTLP_ENDPOINT`, el API funciona igual pero **no exporta**
> telemetría (cero ruido). La instrumentación solo se activa cuando esa variable existe.

## 3. Generar tráfico y ver los 3 pilares
```bash
# Genera algunas requests (ajusta el endpoint a uno real de tu API)
curl http://localhost:8080/swagger/index.html
curl http://localhost:8080/api/Producto
curl http://localhost:8080/healthcheck
```
En Grafana:
- **Métricas** → dashboard *Golden Signals* (ver detalle abajo).
- **Trazas** → **Jaeger UI** (http://localhost:16686, servicio `api-solution-test-ventas`) o
  desde Grafana → Explore → datasource **Jaeger**. Verás el recorrido request → controller → **EF Core/SQL**.
- **Logs** → Explore → datasource **Loki** → `{service_name="api-solution-test-ventas"}` (o `{container="net-api-sales"}`).
  Cada log es JSON con su `trace_id`: clic en "Ver traza" salta a Jaeger (correlación log↔traza).

## Dashboard: Golden Signals (RED + Saturation)
`grafana/provisioning/dashboards/json/api-red.json`. Organizado en **4 filas, una por
cada Golden Signal** de Google SRE:

| Fila | Paneles |
|---|---|
| 🟦 **Tráfico** (Traffic) | req/s total, por método HTTP, por ruta, y llamadas salientes (HttpClient) |
| 🟥 **Errores** (Errors) | % 5xx, % 4xx, requests por código de estado, excepciones .NET, errores en llamadas salientes |
| 🟨 **Latencia** (Latency) | p95, p50/p95/p99, p95 por ruta, p95 en llamadas salientes |
| 🟩 **Saturación** (Saturation) | requests activas, memoria .NET, thread pool (hilos/cola), CPU del proceso, CPU y memoria del host |

## Alertas
Reglas en `prometheus/alert-rules.yml` (las evalúa Prometheus), enrutadas por `alertmanager.yml`:
- **App (RED):** `HighApiErrorRate` (>5% de 5xx), `HighApiLatencyP95` (p95 > 1s).
- **Infra:** `HighCPUUsage` (80%) / `CriticalCPUUsage` (95%), `HighMemoryUsage` (85%), `DiskSpaceLow` (<15%).
- **Disponibilidad:** `TargetDown` (`up == 0`).

Estado de las alertas: Prometheus → *Alerts* (http://localhost:9090/alerts) o el propio
Alertmanager (http://localhost:9093). Los `receivers` son webhooks de ejemplo
(`example.com`) → reemplázalos por Slack/Teams/PagerDuty en producción.

## 4. Apagar
```bash
cd observability && docker compose down          # conserva datos (volúmenes)
cd observability && docker compose down -v       # borra también los datos
```

## ¿Qué se instrumentó en el código?
- `src/ApiSolutionTestVentas.Api/Observability/ObservabilityExtensions.cs` — configura OpenTelemetry
  (métricas ASP.NET Core + HttpClient + Runtime; trazas ASP.NET Core + HttpClient + EF Core) → OTLP al Collector.
- `Observability/ActivityTraceEnricher.cs` — inyecta `trace_id`/`span_id` en cada log (para correlación).
- `Program.cs` — llama a `builder.AddObservability()` y configura Serilog para escribir **JSON al stdout**
  (lo recoge Promtail → Loki). Los logs **ya no** salen por OTLP.
- `ApiSolutionTestVentas.Api.csproj` — paquetes OpenTelemetry (sin `Serilog.Sinks.OpenTelemetry`).

> **Nota de alineación al lab:** se cambió **Tempo → Jaeger** (trazas) y **OTLP de logs → Promtail**
> (logs por stdout). Las métricas y trazas siguen usando OpenTelemetry + el OTel Collector.

## Notas
Métricas clave (nombres en Prometheus):
- **HTTP server:** `http_server_request_duration_seconds_*` (histograma → rate/quantile), `http_server_active_requests`.
- **HTTP client (salientes):** `http_client_request_duration_seconds_*`.
- **Runtime .NET:** `dotnet_process_memory_working_set_bytes`, `dotnet_process_cpu_time_seconds_total`,
  `dotnet_thread_pool_thread_count`, `dotnet_thread_pool_queue_length`, `dotnet_exceptions_total`.
- **Infra (Node Exporter):** `node_cpu_seconds_total`, `node_memory_*`, `node_filesystem_*`.

> Los nombres exactos de las métricas `dotnet_*` dependen de la versión del paquete de
> instrumentación. Si algún panel de la fila *Saturación* aparece "No data", confirma el
> nombre real en Grafana → **Explore** (o Prometheus, escribiendo `dotnet_`) y ajusta el `expr`.

- El nombre del servicio se controla con `OTEL_SERVICE_NAME` (label `service_name` en Prometheus).
- Producción (AKS/Azure): ver `observability/AZURE.md` y `k8s/observability/README.md`.
