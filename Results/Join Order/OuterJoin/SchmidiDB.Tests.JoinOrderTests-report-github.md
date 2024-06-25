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
| **ExecuteJoinTree** | **b, a, n, c, co**  | **1.966 s** | **0.0366 s** | **0.0342 s** |
| **ExecuteJoinTree** | **n, a, b, c, co**  | **1.714 s** | **0.0174 s** | **0.0155 s** |
