#!/usr/bin/env bash
echo "RUNTIME_METRICS:"
curl -s http://localhost:9090/api/v1/label/__name__/values \
  | python3 -c 'import sys,json;[print("  ",n) for n in json.load(sys.stdin)["data"] if any(k in n for k in ["dotnet","kestrel","process_runtime","aspnetcore","gc_"])]'
echo "CPU_5m:"
curl -s --data-urlencode 'query=100 - (avg(rate(node_cpu_seconds_total{mode="idle"}[5m])) * 100)' http://localhost:9090/api/v1/query \
  | python3 -c 'import sys,json;r=json.load(sys.stdin)["data"]["result"];print("  ",r[0]["value"][1] if r else "na")'
echo "ALERTS:"
curl -s http://localhost:9090/api/v1/alerts \
  | python3 -c 'import sys,json;a=json.load(sys.stdin)["data"]["alerts"];[print("  ",x["labels"]["alertname"],x["state"]) for x in a] or print("   ninguna")'
