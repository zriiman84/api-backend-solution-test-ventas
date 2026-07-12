#!/usr/bin/env bash
# Trafico ligero (sin estres de CPU) para mantener el dashboard con datos.
END=$((SECONDS + ${1:-180}))
while [ $SECONDS -lt $END ]; do
  curl -s -o /dev/null http://localhost/
  curl -s -o /dev/null http://localhost/api/Producto
  curl -s -o /dev/null http://localhost/api/Categoria
  curl -s -o /dev/null http://localhost:8080/healthcheck
  sleep 1
done
echo trafico_ligero_fin
