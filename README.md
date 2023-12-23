# FTSBenchmark
Simple .net project to execute several different set of benchmarks.
The idea came from the problem that we run into the performance issue and had
to design a proposal with the strategy how to do it better.

Initial research showed that there are many options, but we wanted to decide
using data, not by feel.

All DB queries should be written and executed in a 'raw-SQL' form to make sure
we understand what's goin on.

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

### Postgres

Manually creating DB:
```
CREATE DATABASE FTSBenchmark;
CREATE SCHEMA public;

CREATE EXTENSION IF NOT EXISTS pg_trgm;
CREATE TABLE IF NOT EXISTS Persons (
   Id SERIAL PRIMARY KEY,
   FirstName VARCHAR(255),
   LastName VARCHAR(255),
   Username VARCHAR(255)
);

CREATE INDEX first_name_trigram_idx ON Persons USING gin(FirstName gin_trgm_ops);
CREATE INDEX last_name_trigram_idx ON Persons USING gin(LastName gin_trgm_ops);
-- drop index if exists trigram_index;


select * from Persons;
```

Scaffolding DB context from existing table:
```
dotnet ef dbcontext scaffold \
  "Server=localhost;Port=5432;Database=ftsbenchmark;User ID=postgres;Password=mysecretpassword" \
  --project src/FTSBenchmark.Infrastructure \
  --startup-project src/FTSBenchmark.WebApi \
  Npgsql.EntityFrameworkCore.PostgreSQL
```

[Trigram indexes](https://www.cockroachlabs.com/docs/stable/trigram-indexes)
[Performance optimisation](https://medium.com/swlh/performance-optimisation-for-wildcards-search-in-postgres-trigram-index-80df0b1f49c7)

## Relevant links

[MariaDB full-text index overview](https://mariadb.com/kb/en/full-text-index-overview/)

### Elastic search

[Getting started with NEST (C#)](https://www.elastic.co/guide/en/elasticsearch/client/net-api/7.17/nest-getting-started.html)
[String querying](https://opster.com/guides/elasticsearch/search-apis/elasticsearch-string-contains-substring/)

## Results

Initial exprimental runs:
```
{
  "Like": {
    "mean": 159.72814199999996,
    "average": 159.72814200000002,
    "percentile10": 83.35816666666666,
    "percentile50": 149.2621,
    "percentile90": 230.3245466666667,
    "averageCount": 5574.54
  },
  "Contains": {
    "mean": 140.22678800000003,
    "average": 140.226788,
    "percentile10": 74.52890333333333,
    "percentile50": 121.78995,
    "percentile90": 237.41573000000025,
    "averageCount": 5574.54
  },
  "MatchAgainst": {
    "mean": 124.37818599999999,
    "average": 124.37818600000003,
    "percentile10": 31.176626666666667,
    "percentile50": 136.22985,
    "percentile90": 215.98322666666672,
    "averageCount": 876.52
  },
  "TrigramPg": {
    "mean": 88.87905200000003,
    "average": 88.87905200000003,
    "percentile10": 60.899973333333335,
    "percentile50": 70.10390000000001,
    "percentile90": 115.5686166666667,
    "averageCount": 5574.54
  }
}
```
