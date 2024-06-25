```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean       | Error    | StdDev   |
|---------------- |---------------- |-----------:|---------:|---------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  | **1,270.7 ms** |  **8.27 ms** |  **7.33 ms** |
| **ExecuteJoinTree** | **b, a, n, c, co**  | **1,295.6 ms** | **11.93 ms** | **10.58 ms** |
| **ExecuteJoinTree** | **b, a, n, co, c**  | **1,327.6 ms** | **11.36 ms** | **10.62 ms** |
| **ExecuteJoinTree** | **b, c, a, n, co**  | **1,191.4 ms** | **16.97 ms** | **15.05 ms** |
| **ExecuteJoinTree** | **co, n, a, b, c**  |   **918.6 ms** | **12.40 ms** | **11.60 ms** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **1,151.4 ms** |  **5.82 ms** |  **5.16 ms** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **1,252.5 ms** | **24.73 ms** | **34.67 ms** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **1,208.4 ms** | **18.57 ms** | **17.37 ms** |
