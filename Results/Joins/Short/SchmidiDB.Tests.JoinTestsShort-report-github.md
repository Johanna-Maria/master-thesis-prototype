```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                      | JoinOperator         | Mean           | Error         | StdDev        |
|---------------------------- |--------------------- |---------------:|--------------:|--------------:|
| **AuthorsJoinNationalities**    | **Hash Join**            | **270,074.264 μs** | **3,407.4636 μs** | **3,020.6277 μs** |
| NationalitiesJoinAuthors    | Hash Join            | 266,494.377 μs | 3,659.1195 μs | 3,422.7427 μs |
| NationalitiesJoinContinents | Hash Join            |       8.395 μs |     0.0400 μs |     0.0375 μs |
| ContinentsJoinNationalities | Hash Join            |       8.500 μs |     0.0347 μs |     0.0325 μs |
| **AuthorsJoinNationalities**    | **Nested-Loop Join**     | **282,150.643 μs** | **2,864.8665 μs** | **2,679.7979 μs** |
| NationalitiesJoinAuthors    | Nested-Loop Join     | 288,961.003 μs | 5,578.2953 μs | 5,478.6319 μs |
| NationalitiesJoinContinents | Nested-Loop Join     |       9.591 μs |     0.0267 μs |     0.0236 μs |
| ContinentsJoinNationalities | Nested-Loop Join     |       9.609 μs |     0.0515 μs |     0.0482 μs |
| **AuthorsJoinNationalities**    | **Neste(...)ndex) [24]** | **272,749.470 μs** | **5,144.7354 μs** | **4,812.3887 μs** |
| NationalitiesJoinAuthors    | Neste(...)ndex) [24] | 266,287.640 μs | 4,990.4101 μs | 4,668.0327 μs |
| NationalitiesJoinContinents | Neste(...)ndex) [24] |       8.138 μs |     0.0419 μs |     0.0350 μs |
| ContinentsJoinNationalities | Neste(...)ndex) [24] |       7.921 μs |     0.0446 μs |     0.0396 μs |
| **AuthorsJoinNationalities**    | **Sort-Merge Join**      | **299,874.818 μs** | **2,888.6548 μs** | **2,560.7173 μs** |
| NationalitiesJoinAuthors    | Sort-Merge Join      | 297,935.117 μs | 4,079.2884 μs | 3,815.7689 μs |
| NationalitiesJoinContinents | Sort-Merge Join      |       8.892 μs |     0.0273 μs |     0.0255 μs |
| ContinentsJoinNationalities | Sort-Merge Join      |       8.936 μs |     0.0457 μs |     0.0427 μs |
