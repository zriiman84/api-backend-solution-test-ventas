#!/usr/bin/env bash
check() {
  local env=$1 ip=$2
  echo "=== $env  ($ip:8080) ==="
  for path in /healthcheck /swagger/index.html; do
    code=$(curl -s -o /dev/null -m 10 -w '%{http_code}' "http://$ip:8080$path" 2>/dev/null)
    if [ "$code" = "000" ]; then estado="SIN RESPUESTA (caido/timeout/IP cambiada)"; else estado="responde"; fi
    echo "  $path -> HTTP $code  ($estado)"
  done
  echo
}
check DEV  20.14.108.133
check QA   4.148.48.74
check PROD 132.196.114.31
