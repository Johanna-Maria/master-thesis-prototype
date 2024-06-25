```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3737/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean    | Error    | StdDev   |
|---------------- |---------------- |--------:|---------:|---------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **2.609 s** | **0.0114 s** | **0.0101 s** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **2.678 s** | **0.0137 s** | **0.0121 s** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **2.743 s** | **0.0155 s** | **0.0145 s** |
| **ExecuteJoinTree** | **c, b, a, n, co**  | **2.127 s** | **0.0113 s** | **0.0106 s** |
| **ExecuteJoinTree** | **co, n, a, b, c**  | **1.509 s** | **0.0116 s** | **0.0108 s** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **2.030 s** | **0.0103 s** | **0.0091 s** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **2.111 s** | **0.0133 s** | **0.0118 s** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **1.917 s** | **0.0119 s** | **0.0111 s** |
