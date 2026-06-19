#!/usr/bin/env bash
echo "CPU_5m:"
curl -s --data-urlencode 'query=100 - (avg(rate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)' http://localhost:9090/api/v1/query \
  | python3 -c 'import sys,json;r=json.load(sys.stdin)["data"]["result"];print("%.1f"%float(r[0]["value"][1]) if r else "na")'
echo "FIRING_OR_PENDING:"
curl -s http://localhost:9090/api/v1/alerts \
  | python3 -c 'import sys,json;a=json.load(sys.stdin)["data"]["alerts"];[print(" ",x["labels"]["alertname"],x["state"]) for x in a] or print("  ninguna")'
echo "yes_procs:"; pgrep -c yes || echo 0
echo "req_count_sample:"
curl -s 'http://localhost:9090/api/v1/query?query=sum(http_server_request_duration_seconds_count)' \
  | python3 -c 'import sys,json;r=json.load(sys.stdin)["data"]["result"];print(r[0]["value"][1] if r else "na")'
