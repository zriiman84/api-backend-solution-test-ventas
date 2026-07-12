#!/usr/bin/env bash
for i in $(seq 1 30); do
  F=$(curl -s http://localhost:9090/api/v1/alerts \
    | python3 -c 'import sys,json;a=[x for x in json.load(sys.stdin)["data"]["alerts"] if x["state"]=="firing"];print(",".join(sorted(set(x["labels"]["alertname"] for x in a))))')
  if [ -n "$F" ]; then echo "FIRING: $F (tras $((i*10))s)"; exit 0; fi
  echo "...esperando ($((i*10))s) - aun sin firing"
  sleep 10
done
echo "TIMEOUT_no_firing"
