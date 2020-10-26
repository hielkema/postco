#!/bin/bash
git pull
docker-compose -f docker-compose_prod.yml build --no-cache
docker-compose -f docker-compose_prod.yml up -d