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
| **ExecuteJoinTree** | **b, a, c, n, co**  | **12.787 s** | **0.0423 s** | **0.0375 s** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **12.891 s** | **0.0427 s** | **0.0399 s** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **13.106 s** | **0.0232 s** | **0.0217 s** |
| **ExecuteJoinTree** | **b, c, a, n, co**  | **11.592 s** | **0.0358 s** | **0.0335 s** |
| **ExecuteJoinTree** | **co, n, a, b, c**  |  **9.020 s** | **0.0270 s** | **0.0239 s** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **11.397 s** | **0.0196 s** | **0.0173 s** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **11.943 s** | **0.0913 s** | **0.0854 s** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **11.014 s** | **0.0171 s** | **0.0151 s** |
