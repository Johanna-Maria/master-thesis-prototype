```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean      | Error     | StdDev    |
|---------------- |---------------- |----------:|----------:|----------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **10.277 ms** | **0.1435 ms** | **0.1120 ms** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **10.572 ms** | **0.0334 ms** | **0.0313 ms** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **10.828 ms** | **0.0709 ms** | **0.0629 ms** |
| **ExecuteJoinTree** | **b, c, a, n, co**  |  **8.994 ms** | **0.0444 ms** | **0.0393 ms** |
| **ExecuteJoinTree** | **co, n, a, b, c**  |  **7.215 ms** | **0.0275 ms** | **0.0230 ms** |
| **ExecuteJoinTree** | **n, a, b, c, co**  |  **9.167 ms** | **0.0429 ms** | **0.0381 ms** |
| **ExecuteJoinTree** | **n, a, b, co, c**  |  **9.612 ms** | **0.0601 ms** | **0.0562 ms** |
| **ExecuteJoinTree** | **n, a, co, b, c**  |  **8.437 ms** | **0.0498 ms** | **0.0466 ms** |
