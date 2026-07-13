# ☁️ Observabilidad en Azure / AKS (fase posterior)

> Estado: **DOCUMENTADO, no aplicado.** Primero validamos en local; esto es el plan para
> llevarlo al clúster AKS. Suscripción activa: **Azure 02** (`b497fd69-266c-46a9-b55b-8be0cd579667`).

El API ya está instrumentado con OpenTelemetry y exporta por **OTLP**. Esa es la gran ventaja:
para producción **no hay que tocar el código**, solo cambiar a dónde apunta el OTLP
(`OTEL_EXPORTER_OTLP_ENDPOINT`) y desplegar el backend de observabilidad en el clúster.

## Contexto de la infra (de `Acciones_CI_CD.txt`)
- Región: `westus3`. ACR compartido: `acrsalesstoredmcshared.azurecr.io`.
- Clústeres AKS por ambiente: `aks-api-salesstore-backend-{dev,qa,prd}`.
- Resource groups: `rg-solutiontestventas-salesstore-dmc-{dev,qa,prd}`.
- Despliegue por GitHub Actions; Service tipo LoadBalancer en puerto 8080.

---

## Opción A — Azure nativo (recomendada para integrarse con el ecosistema Azure)

Usa servicios gestionados; menos que mantener.

| Pilar | Servicio Azure |
|---|---|
| Métricas | **Azure Monitor managed Prometheus** (addon de AKS) |
| Trazas + Logs de app | **Application Insights** (compatible OTLP/OpenTelemetry) |
| Visualización | **Azure Managed Grafana** (se conecta a managed Prometheus y App Insights) |

**Flujo:** API → OTLP → **OTel Collector en AKS** → exporter `azuremonitor` (App Insights) +
los pods publican métricas que el addon de managed Prometheus scrapea.

Terraform a añadir (en `iac-terraform-backend-test-ventas`, provider azurerm):
```hcl
resource "azurerm_application_insights" "api" {
  name                = "appi-salesstore-${var.environment}"
  location            = var.location
  resource_group_name = var.resource_group_name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.main.id
}

resource "azurerm_monitor_workspace" "prom" {       # managed Prometheus
  name                = "amw-salesstore-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
}

resource "azurerm_dashboard_grafana" "grafana" {    # managed Grafana
  name                = "amg-salesstore-${var.environment}"
  resource_group_name = var.resource_group_name
  location            = var.location
  grafana_major_version = 11
  azure_monitor_workspace_integrations { resource_id = azurerm_monitor_workspace.prom.id }
}
```
Y habilitar el addon de métricas en el AKS:
```hcl
resource "azurerm_kubernetes_cluster" "aks" {
  # ... config existente ...
  monitor_metrics {}   # habilita managed Prometheus scraping
}
```

---

## Opción B — Stack self-hosted en AKS (idéntico a tu local, control total)

Reproduce en el clúster lo que ya tienes en `docker-compose.yml`, vía Helm:
```bash
# Prometheus + Grafana
helm install kube-prometheus prometheus-community/kube-prometheus-stack -n observability --create-namespace
# Trazas
helm install tempo grafana/tempo -n observability
# Logs
helm install loki grafana/loki -n observability
# Collector
helm install otel-collector open-telemetry/opentelemetry-collector -n observability -f otel-values.yaml
```
Ventaja: mismas herramientas que aprendiste; portable a cualquier nube. Desventaja: tú
mantienes el almacenamiento y la disponibilidad.

---

## Cambios en `k8s/deployment.yml` (común a A y B)
Añadir al contenedor del API las variables OTEL apuntando al Collector del clúster:
```yaml
          env:
            - name: OTEL_EXPORTER_OTLP_ENDPOINT
              value: "http://otel-collector.observability.svc.cluster.local:4317"
            - name: OTEL_SERVICE_NAME
              value: "api-solution-test-ventas"
            - name: OTEL_RESOURCE_ATTRIBUTES
              value: "deployment.environment=$(ASPNETCORE_ENVIRONMENT)"
```
Y, si usas managed Prometheus con scraping directo, anotar el Service/Pod:
```yaml
  annotations:
    prometheus.io/scrape: "true"
    prometheus.io/port: "8889"
```

## Recomendación
Para tu caso (ya estás en Azure, con AKS y Terraform), la **Opción A** es la más coherente:
menos mantenimiento y se integra con lo que ya tienes. Cuando confirmemos la suscripción
activa y quieras avanzar, lo implementamos por ambiente empezando por **dev**.
