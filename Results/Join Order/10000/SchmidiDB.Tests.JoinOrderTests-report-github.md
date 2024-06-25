```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean      | Error    | StdDev   |
|---------------- |---------------- |----------:|---------:|---------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **127.71 ms** | **2.439 ms** | **2.395 ms** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **124.94 ms** | **1.951 ms** | **1.825 ms** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **129.46 ms** | **2.012 ms** | **1.882 ms** |
| **ExecuteJoinTree** | **b, c, a, n, co**  | **117.97 ms** | **1.140 ms** | **1.011 ms** |
| **ExecuteJoinTree** | **co, n, a, b, c**  |  **89.36 ms** | **1.686 ms** | **1.577 ms** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **117.45 ms** | **0.908 ms** | **0.850 ms** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **121.00 ms** | **1.495 ms** | **1.326 ms** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **105.05 ms** | **0.893 ms** | **0.792 ms** |
