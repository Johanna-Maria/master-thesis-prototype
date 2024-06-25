```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3737/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method          | JoinTreeMeasure | Mean       | Error    | StdDev   |
|---------------- |---------------- |-----------:|---------:|---------:|
| **ExecuteJoinTree** | **b, a, c, n, co**  |   **303.1 ms** |  **0.81 ms** |  **0.72 ms** |
| **ExecuteJoinTree** | **b, a, n, c, co**  |   **312.5 ms** |  **1.60 ms** |  **1.49 ms** |
| **ExecuteJoinTree** | **b, a, n, co, c**  |   **302.9 ms** |  **1.03 ms** |  **0.96 ms** |
| **ExecuteJoinTree** | **b, c, a, n, co**  |   **309.0 ms** |  **2.11 ms** |  **1.97 ms** |
| **ExecuteJoinTree** | **co, n, a, b, c**  |   **943.8 ms** | **10.72 ms** | **10.03 ms** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **1,164.7 ms** |  **7.00 ms** |  **6.20 ms** |
| **ExecuteJoinTree** | **n, a, b, co, c**  | **1,202.8 ms** |  **8.75 ms** |  **8.19 ms** |
| **ExecuteJoinTree** | **n, a, co, b, c**  | **1,132.1 ms** |  **7.83 ms** |  **6.94 ms** |
