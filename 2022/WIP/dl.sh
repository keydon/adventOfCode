#!/bin/bash
curl 'https://adventofcode.com/2020/day/'$(date +%-d)'/input' \
  -H 'authority: adventofcode.com' \
  -H 'cache-control: max-age=0' \
  -H 'dnt: 1' \
  -H 'upgrade-insecure-requests: 1' \
  -H 'user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36' \
  -H 'accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9' \
  -H 'sec-fetch-site: same-origin' \
  -H 'sec-fetch-mode: navigate' \
  -H 'sec-fetch-user: ?1' \
  -H 'sec-fetch-dest: document' \
  -H 'referer: https://adventofcode.com/2020/day/1' \
  -H 'accept-language: en-US,en;q=0.9,de-DE;q=0.8,de;q=0.7' \
  -H 'cookie: session=53616c7465645f5ff5187331eb985ef893df827cdc99666dcfe384b266e379347bdf7bdad1638eb9b1a09c17c0a579df' \
  --compressed --output input.txt

  head -n 5 input.txt
  echo "..."
  tail -n 5 input.txt