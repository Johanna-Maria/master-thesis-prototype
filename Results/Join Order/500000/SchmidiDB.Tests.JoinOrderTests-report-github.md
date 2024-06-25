```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean    | Error    | StdDev   |
|---------------- |---------------- |--------:|---------:|---------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **6.403 s** | **0.0228 s** | **0.0202 s** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **6.699 s** | **0.1302 s** | **0.1825 s** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **6.739 s** | **0.0240 s** | **0.0225 s** |
| **ExecuteJoinTree** | **b, c, a, n, co**  | **5.833 s** | **0.0405 s** | **0.0379 s** |
| **ExecuteJoinTree** | **co, n, a, b, c**  | **4.607 s** | **0.0222 s** | **0.0197 s** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **5.716 s** | **0.0217 s** | **0.0193 s** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **5.929 s** | **0.0142 s** | **0.0118 s** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **5.463 s** | **0.0198 s** | **0.0185 s** |
