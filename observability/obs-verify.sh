#!/usr/bin/env bash
cd "$(dirname "$0")"
echo "=== Docker ==="
docker version --format 'Server: {{.Server.Version}}'
echo
echo "=== Validacion de docker-compose.yml ==="
if docker compose config --quiet; then echo "COMPOSE OK"; else echo "COMPOSE INVALIDO"; exit 1; fi
echo
echo "=== Carpeta ==="
pwd
