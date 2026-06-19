# 🔭 Observabilidad local — API solution-test-ventas

Stack local de los **3 pilares** (métricas, trazas, logs) para el API .NET, basado en
**OpenTelemetry + Prometheus + Tempo + Loki + Grafana**.

```
API .NET ──OTLP(gRPC :4317)──► OpenTelemetry Collector ──► Prometheus  (métricas)
                                                       ├──► Tempo       (trazas)
                                                       └──► Loki        (logs)
                                                              todo en Grafana (:3000)
```

## Requisitos
- Docker + Docker Compose (en este equipo: dentro de WSL Ubuntu).
- El stack del **laboratorio** (`obserbabilidad/prometheus-lab`) usa los puertos 9090/3000.
  Asegúrate de que esté detenido antes (`docker compose stop` en esa carpeta) para no chocar.

## 1. Levantar el stack de observabilidad
```bash
cd observability
docker compose up -d
docker compose ps          # los 5 servicios deben estar Up
```

| Servicio | URL local | Para qué |
|---|---|---|
| Grafana | http://localhost:3000 (admin/admin) | Ver todo (dashboards, trazas, logs) |
| Prometheus | http://localhost:9090 | Consultar métricas crudas |
| Tempo | http://localhost:3200 | API de trazas (se usa desde Grafana) |
| Loki | http://localhost:3100 | API de logs (se usa desde Grafana) |
| OTel Collector | OTLP en :4317 (gRPC) y :4318 (HTTP) | Recibe la telemetría del API |

Datasources y un dashboard **RED** ("API solution-test-ventas - RED") quedan provisionados
automáticamente en Grafana (carpeta *API solution-test-ventas*).

## 2. Levantar el API conectado a la observabilidad
Desde la raíz del repo del backend (necesitas un `.env` con credenciales de Azure SQL — ver `.env.example`):
```bash
docker compose -f docker-compose.yaml -f docker-compose.observability.yml up -d --build
```
Esto añade al contenedor del API las variables:
- `OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4317`
- `OTEL_SERVICE_NAME=api-solution-test-ventas`

y lo une a la red `observability`. El API queda en http://localhost:8080.

> Si NO defines `OTEL_EXPORTER_OTLP_ENDPOINT`, el API funciona igual pero **no exporta**
> telemetría (cero ruido). La instrumentación solo se activa cuando esa variable existe.

## 3. Generar tráfico y ver los 3 pilares
```bash
# Genera algunas requests (ajusta el endpoint a uno real de tu API)
curl http://localhost:8080/swagger/index.html
curl http://localhost:8080/api/productos
```
En Grafana:
- **Métricas** → dashboard *RED*: tasa, % de errores, latencia p50/p95/p99, requests por ruta/estado.
- **Trazas** → Explore → datasource **Tempo** → busca por servicio `api-solution-test-ventas`.
  Verás el recorrido request → controller → **EF Core/SQL**.
- **Logs** → Explore → datasource **Loki** → `{service_name="api-solution-test-ventas"}`.
  Cada log lleva su `trace_id`: clic en "Ver traza" salta a Tempo (correlación log↔traza).

## 4. Apagar
```bash
cd observability && docker compose down          # conserva datos (volúmenes)
cd observability && docker compose down -v       # borra también los datos
```

## ¿Qué se instrumentó en el código?
- `src/ApiSolutionTestVentas.Api/Observability/ObservabilityExtensions.cs` — configura OpenTelemetry
  (métricas ASP.NET Core + HttpClient + Runtime; trazas ASP.NET Core + HttpClient + EF Core).
- `Program.cs` — llama a `builder.AddObservability()` y añade el sink OTLP a Serilog (logs → Loki).
- `ApiSolutionTestVentas.Api.csproj` — paquetes OpenTelemetry + `Serilog.Sinks.OpenTelemetry`.

## Notas
- Métricas clave (nombres en Prometheus): `http_server_request_duration_seconds_*`
  (histograma → rate/quantile), `http_server_active_requests`, métricas de runtime `dotnet_*`.
- El nombre del servicio se controla con `OTEL_SERVICE_NAME`.
- Producción (AKS/Azure): ver `observability/AZURE.md`.
