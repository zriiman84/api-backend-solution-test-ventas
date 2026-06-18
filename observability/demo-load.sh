#!/usr/bin/env bash
# Genera carga de CPU (para disparar HighCPUUsage) + trafico al API (para RED/trazas/logs).
DURACION=${1:-360}
N=$(nproc)
echo "Carga CPU en $N cores + trafico durante ${DURACION}s..."
for i in $(seq "$N"); do yes > /dev/null & done
YESPIDS=$(jobs -p)

END=$((SECONDS + DURACION))
while [ $SECONDS -lt $END ]; do
  curl -s -o /dev/null http://localhost/
  curl -s -o /dev/null http://localhost/api/Producto
  curl -s -o /dev/null http://localhost/api/Categoria
  curl -s -o /dev/null http://localhost/api/Cliente
  curl -s -o /dev/null http://localhost:8080/healthcheck
  curl -s -o /dev/null http://localhost:8080/swagger/index.html
  sleep 1
done

kill $YESPIDS 2>/dev/null
wait 2>/dev/null
echo "carga completada"
