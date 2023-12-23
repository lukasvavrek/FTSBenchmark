#!/usr/bin/env bash

docker run --name fts-postgres \
    -e POSTGRES_PASSWORD=mysecretpassword \
    -p 5432:5432 \
    -d postgres
