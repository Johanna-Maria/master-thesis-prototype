```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                    | JoinOperator         | Mean                 | Error              | StdDev             |
|-------------------------- |--------------------- |---------------------:|-------------------:|-------------------:|
| **AuthorsJoinBooks**          | **Hash Join**            | **2,667,133,271.429 ns** | **19,239,708.0057 ns** | **17,055,500.0111 ns** |
| BooksJoinAuthors          | Hash Join            | 2,585,363,020.000 ns | 12,447,510.1113 ns | 11,643,408.6811 ns |
| AuthorsJoinBooksOrdered   | Hash Join            | 2,492,650,800.000 ns |  9,883,344.0938 ns |  8,761,327.1081 ns |
| BooksJoinAuthorsOrdered   | Hash Join            | 2,554,159,533.333 ns | 11,791,955.1794 ns | 11,030,202.1911 ns |
| AuthorsJoinBooksUnordered | Hash Join            | 2,654,049,346.667 ns |  8,943,358.6939 ns |  8,365,623.2711 ns |
| BooksJoinAuthorsUnordered | Hash Join            | 2,636,083,613.333 ns | 13,110,953.8900 ns | 12,263,994.4033 ns |
| **AuthorsJoinBooks**          | **Neste(...)ndex) [24]** | **2,436,141,180.000 ns** | **13,727,661.3215 ns** | **12,840,862.9174 ns** |
| BooksJoinAuthors          | Neste(...)ndex) [24] | 2,505,514,340.000 ns | 14,005,312.6145 ns | 13,100,578.1092 ns |
| AuthorsJoinBooksOrdered   | Neste(...)ndex) [24] |             1.331 ns |          0.0071 ns |          0.0063 ns |
| BooksJoinAuthorsOrdered   | Neste(...)ndex) [24] | 2,349,230,200.000 ns | 15,913,720.6440 ns | 13,288,677.4738 ns |
| AuthorsJoinBooksUnordered | Neste(...)ndex) [24] | 2,442,756,550.000 ns | 13,641,264.0869 ns | 12,092,625.2996 ns |
| BooksJoinAuthorsUnordered | Neste(...)ndex) [24] |             1.297 ns |          0.0051 ns |          0.0048 ns |
| **AuthorsJoinBooks**          | **Sort-Merge Join**      | **2,699,285,380.000 ns** | **11,583,634.6325 ns** | **10,835,339.0223 ns** |
| BooksJoinAuthors          | Sort-Merge Join      | 2,723,506,760.000 ns | 11,538,000.8309 ns | 10,792,653.1359 ns |
| AuthorsJoinBooksOrdered   | Sort-Merge Join      | 2,367,954,540.000 ns | 11,144,805.4681 ns | 10,424,857.9495 ns |
| BooksJoinAuthorsOrdered   | Sort-Merge Join      | 2,390,025,026.667 ns | 15,582,775.8384 ns | 14,576,138.1875 ns |
| AuthorsJoinBooksUnordered | Sort-Merge Join      | 2,890,031,240.000 ns | 17,830,106.8838 ns | 16,678,293.0417 ns |
| BooksJoinAuthorsUnordered | Sort-Merge Join      | 2,910,033,800.000 ns | 16,184,411.1690 ns | 15,138,908.2490 ns |
