```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3737/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean    | Error   | StdDev  |
|---------------- |---------------- |--------:|--------:|--------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **27.10 s** | **0.083 s** | **0.077 s** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **26.83 s** | **0.073 s** | **0.068 s** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **27.92 s** | **0.083 s** | **0.077 s** |
| **ExecuteJoinTree** | **c, b, a, n, co**  | **21.75 s** | **0.055 s** | **0.049 s** |
| **ExecuteJoinTree** | **co, n, a, b, c**  | **14.88 s** | **0.030 s** | **0.026 s** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **20.16 s** | **0.041 s** | **0.038 s** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **20.72 s** | **0.058 s** | **0.054 s** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **18.68 s** | **0.042 s** | **0.039 s** |
