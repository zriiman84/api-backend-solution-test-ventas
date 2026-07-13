#!/usr/bin/env bash
set -euo pipefail
# =====================================================================
# Instala el stack de observabilidad (Opción B, self-hosted) en AKS.
# Se ejecuta UNA SOLA VEZ por clúster/ambiente (no en cada push de la app).
#
# PRE-REQUISITOS:
#   1) Sesión Azure con acceso a la suscripción 2 y al AKS del ambiente.
#   2) kubectl apuntando al clúster:
#        az aks get-credentials \
#          --resource-group rg-solutiontestventas-salesstore-dmc-<env> \
#          --name aks-api-salesstore-backend-<env>
#   3) helm instalado.
# =====================================================================
cd "$(dirname "$0")"

helm repo add open-telemetry https://open-telemetry.github.io/opentelemetry-helm-charts
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo add grafana https://grafana.github.io/helm-charts
helm repo update

kubectl create namespace observability --dry-run=client -o yaml | kubectl apply -f -

# 1) Backends de telemetría primero
helm upgrade --install kube-prometheus-stack prometheus-community/kube-prometheus-stack \
  -n observability -f kube-prometheus-stack-values.yaml --wait
helm upgrade --install tempo grafana/tempo -n observability -f tempo-values.yaml --wait
helm upgrade --install loki  grafana/loki  -n observability -f loki-values.yaml  --wait

# 2) OTel Collector (recibe OTLP del API y reparte)
helm upgrade --install otel-collector open-telemetry/opentelemetry-collector \
  -n observability -f otel-collector-values.yaml --wait

kubectl -n observability get pods
echo ""
echo "Grafana (port-forward):"
echo "  kubectl -n observability port-forward svc/kube-prometheus-stack-grafana 3000:80"
echo "  -> http://localhost:3000 (admin / el adminPassword de kube-prometheus-stack-values.yaml)"
