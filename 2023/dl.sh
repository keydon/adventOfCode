#!/bin/bash
curl 'https://adventofcode.com/2023/day/3/input' \
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
  -H 'cookie: session=53616c7465645f5ff81453c31ee7233a2124060ca429b11d2fdd4b6f01357f79d5caf835308f8b502e06cafbbf7dba593eb3c5a4c88c6fa7693d5b682635dd93' \
  --compressed --output input.txt

  head -n 5 input.txt
  echo "..."
  tail -n 5 input.txt