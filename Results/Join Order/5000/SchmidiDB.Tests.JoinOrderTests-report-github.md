```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean     | Error    | StdDev   |
|---------------- |---------------- |---------:|---------:|---------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **62.04 ms** | **1.181 ms** | **1.264 ms** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **63.24 ms** | **0.972 ms** | **0.910 ms** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **65.22 ms** | **1.251 ms** | **1.441 ms** |
| **ExecuteJoinTree** | **b, c, a, n, co**  | **57.94 ms** | **1.001 ms** | **0.936 ms** |
| **ExecuteJoinTree** | **co, n, a, b, c**  | **43.63 ms** | **0.698 ms** | **0.653 ms** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **58.92 ms** | **1.134 ms** | **1.305 ms** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **60.38 ms** | **0.873 ms** | **0.682 ms** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **52.06 ms** | **0.754 ms** | **0.668 ms** |
