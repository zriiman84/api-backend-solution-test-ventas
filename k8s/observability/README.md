# Observabilidad en AKS — Opción B (self-hosted)

Despliega en el clúster el mismo stack que usamos en local (alineado al lab DMC):
**OTel Collector → Prometheus + Jaeger + Loki**, con **Promtail** para logs y **Grafana**,
todo en el namespace `observability`, vía Helm.

```
API (.NET, namespace default) ──OTLP:4317──▶ OTel Collector (ns observability)
                                              ├─ remote_write ─▶ Prometheus (kube-prometheus-stack)
                                              └─ OTLP ─────────▶ Jaeger (all-in-one)
API (.NET, stdout) ──▶ Promtail (DaemonSet) ──────────────────▶ Loki
                                                       todo en Grafana (kube-prometheus-stack)
```

## Dos partes (importante)
1. **El stack de observabilidad** (este directorio) → se instala **una sola vez** por clúster con `install.sh` (Helm). NO se redepliega en cada push.
2. **El cableado de la app** → las variables `OTEL_*` en `../deployment.yml`. Eso **sí** se aplica con un **commit + push** a la rama del ambiente (el pipeline hace `kubectl apply`).

## Despliegue (una vez, cuando tengas acceso)
```bash
# Conéctate al clúster del ambiente (ej. dev)
az aks get-credentials \
  --resource-group rg-solutiontestventas-salesstore-dmc-dev \
  --name aks-api-salesstore-backend-dev

cd k8s/observability
bash install.sh
```

## Ver Grafana
```bash
kubectl -n observability port-forward svc/kube-prometheus-stack-grafana 3000:80
# http://localhost:3000  (usuario admin / adminPassword del values)
```
Datasources ya provisionados: Prometheus (métricas), Jaeger (trazas), Loki (logs), correlados por `trace_id`.

Jaeger UI (opcional, sin datasource):
```bash
kubectl -n observability port-forward svc/jaeger 16686:16686   # http://localhost:16686
```

## Archivos
| Archivo | Qué configura |
|---|---|
| `otel-collector-values.yaml` | Collector (imagen contrib): recibe OTLP, reparte métricas → Prometheus y trazas → Jaeger |
| `kube-prometheus-stack-values.yaml` | Prometheus (remote-write on) + Grafana (+ datasources Jaeger/Loki) + Alertmanager |
| `jaeger.yaml` | Jaeger all-in-one (Deployment+Service, in-memory) — backend de trazas |
| `loki-values.yaml` | Loki single-binary (filesystem) |
| `promtail-values.yaml` | Promtail (DaemonSet): recoge el stdout de los pods → Loki |
| `install.sh` | Añade repos Helm, aplica `jaeger.yaml` e instala todo en orden |

## A AJUSTAR antes de producción (no probado en clúster real aún)
- **`storageClassName`**: descomentar y poner el de tu AKS (p.ej. `managed-csi`) en los PVC de Prometheus/Grafana/Loki.
- **`grafana.adminPassword`**: hoy es `CHANGE_ME_admin` → usar un secreto real (`existingSecret`) o Azure Key Vault.
- **Recursos** (`requests`/`limits`): dimensionar según el node pool (hoy `Standard_B2s`, modesto).
- **Persistencia/retención**: 7 días y discos de ejemplo; ajustar a la política real.
- **Exposición de Grafana**: hoy solo por `port-forward`. Para acceso permanente, Ingress + cert (no expongas Grafana sin auth/TLS).
- **Alta disponibilidad**: Loki va en single-binary y **Jaeger en all-in-one con storage in-memory**
  (las trazas se pierden al reiniciar el pod). Suficiente para dev/demo; no HA ni persistente.
  Para trazas persistentes: Jaeger con Cassandra/Elasticsearch, o volver a Tempo.

> Alternativa gestionada (menos mantenimiento): Opción A en `../../observability/AZURE.md` (Azure Monitor + Managed Prometheus/Grafana).
