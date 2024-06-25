```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean     | Error   | StdDev  |
|---------------- |---------------- |---------:|--------:|--------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **633.4 ms** | **3.00 ms** | **2.66 ms** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **640.5 ms** | **8.01 ms** | **7.49 ms** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **663.0 ms** | **3.59 ms** | **3.19 ms** |
| **ExecuteJoinTree** | **b, c, a, n, co**  | **590.7 ms** | **6.06 ms** | **5.67 ms** |
| **ExecuteJoinTree** | **co, n, a, b, c**  | **410.8 ms** | **2.40 ms** | **2.13 ms** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **584.6 ms** | **6.28 ms** | **5.87 ms** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **618.3 ms** | **6.93 ms** | **6.48 ms** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **544.1 ms** | **7.20 ms** | **6.73 ms** |
