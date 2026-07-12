#!/usr/bin/env bash
for ip in 20.14.108.133:DEV 4.148.48.74:QA 132.196.114.31:PROD; do
  addr=${ip%%:*}; env=${ip##*:}
  echo "===== $env ($addr) ====="
  curl -s -m 10 "http://$addr:8080/healthcheck"
  echo; echo
done
