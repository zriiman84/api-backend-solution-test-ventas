# Observabilidad en AKS — Opción B (self-hosted)

Despliega en el clúster el mismo stack que usamos en local: **OTel Collector → Prometheus + Tempo + Loki → Grafana**, todo en el namespace `observability`, vía Helm.

```
API (.NET, namespace default) ──OTLP:4317──▶ OTel Collector (ns observability)
                                              ├─ remote_write ─▶ Prometheus (kube-prometheus-stack)
                                              ├─ OTLP ─────────▶ Tempo
                                              └─ OTLP/HTTP ────▶ Loki
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
Datasources ya provisionados: Prometheus (métricas), Tempo (trazas), Loki (logs), correlados por `trace_id`.

## Archivos
| Archivo | Qué configura |
|---|---|
| `otel-collector-values.yaml` | Collector (imagen contrib): recibe OTLP, reparte a Prometheus/Tempo/Loki |
| `kube-prometheus-stack-values.yaml` | Prometheus (remote-write on) + Grafana (+ datasources Tempo/Loki) + Alertmanager |
| `tempo-values.yaml` | Tempo single-binary (OTLP, storage local) |
| `loki-values.yaml` | Loki single-binary (filesystem, OTLP) |
| `install.sh` | Añade repos Helm e instala todo en orden |

## A AJUSTAR antes de producción (no probado en clúster real aún)
- **`storageClassName`**: descomentar y poner el de tu AKS (p.ej. `managed-csi`) en los PVC de Prometheus/Grafana/Tempo/Loki.
- **`grafana.adminPassword`**: hoy es `CHANGE_ME_admin` → usar un secreto real (`existingSecret`) o Azure Key Vault.
- **Recursos** (`requests`/`limits`): dimensionar según el node pool (hoy `Standard_B2s`, modesto).
- **Persistencia/retención**: 7 días y discos de ejemplo; ajustar a la política real.
- **Exposición de Grafana**: hoy solo por `port-forward`. Para acceso permanente, Ingress + cert (no expongas Grafana sin auth/TLS).
- **Alta disponibilidad**: Tempo/Loki van en single-binary (suficiente para empezar; no HA).

> Alternativa gestionada (menos mantenimiento): Opción A en `../../observability/AZURE.md` (Azure Monitor + Managed Prometheus/Grafana).
