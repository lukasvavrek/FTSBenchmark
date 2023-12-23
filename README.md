# FTSBenchmark
Simple .net project to execute several different set of benchmarks.
The idea came from the problem that we run into the performance issue and had
to design a proposal with the strategy how to do it better.

Initial research showed that there are many options, but we wanted to decide
using data, not by feel.

All DB queries should be written and executed in a 'raw-SQL' form to make sure
we understand what's goin on.

## TODO

* setup `ConnectionString` to point to MariaDB
* write search handler that uses `LIKE` (existing solution)
* write search handler that uses `MATCH` full-text index
* write handler that uses both and presents results

## Notes

### DB Migrations

Create new migration:
```
dotnet ef migrations add "Initial-schema" \
    --project src/FTSBenchmark.Infrastructure \
    --startup-project src/FTSBenchmark.WebApi \
    --output-dir Database/Migrations
```

Apply migrations to DB:
```
dotnet ef database update \
    --project src/FTSBenchmark.Infrastructure \
    --startup-project src/FTSBenchmark.WebApi
```

## Relevant links

[MariaDB full-text index overview](https://mariadb.com/kb/en/full-text-index-overview/)

### Elastic search

[Getting started with NEST (C#)](https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/nest-getting-started.html)
[String querying](https://opster.com/guides/elasticsearch/search-apis/elasticsearch-string-contains-substring/)
