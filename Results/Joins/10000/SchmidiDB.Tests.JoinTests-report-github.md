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
| **AuthorsJoinBooks**          | **Hash Join**            |    **56,660,316.374 ns** |  **1,067,730.4596 ns** |  **1,186,779.5768 ns** |
| BooksJoinAuthors          | Hash Join            |    54,177,560.131 ns |  1,070,701.7430 ns |  1,728,983.7355 ns |
| AuthorsJoinBooksOrdered   | Hash Join            |    53,422,949.550 ns |  1,045,130.7769 ns |  1,774,711.3770 ns |
| BooksJoinAuthorsOrdered   | Hash Join            |    54,611,500.855 ns |  1,068,023.8243 ns |  1,870,557.5744 ns |
| AuthorsJoinBooksUnordered | Hash Join            |    55,699,418.621 ns |  1,087,222.8605 ns |  1,593,637.1721 ns |
| BooksJoinAuthorsUnordered | Hash Join            |    55,499,879.762 ns |  1,094,705.4038 ns |  1,569,993.7788 ns |
| **AuthorsJoinBooks**          | **Nested-Loop Join**     | **3,011,838,164.286 ns** | **11,439,873.8542 ns** | **10,141,150.1978 ns** |
| BooksJoinAuthors          | Nested-Loop Join     | 2,978,447,453.333 ns |  7,993,335.0078 ns |  7,476,970.5257 ns |
| AuthorsJoinBooksOrdered   | Nested-Loop Join     | 4,017,049,338.462 ns | 11,727,673.3564 ns |  9,793,138.4016 ns |
| BooksJoinAuthorsOrdered   | Nested-Loop Join     | 3,059,027,366.667 ns | 19,468,385.5943 ns | 18,210,739.9639 ns |
| AuthorsJoinBooksUnordered | Nested-Loop Join     | 3,059,741,166.667 ns | 22,443,928.8234 ns | 20,994,064.9466 ns |
| BooksJoinAuthorsUnordered | Nested-Loop Join     | 3,667,398,313.333 ns | 15,768,012.5261 ns | 14,749,408.7001 ns |
| **AuthorsJoinBooks**          | **Neste(...)ndex) [24]** |    **50,966,336.429 ns** |    **915,635.1148 ns** |    **811,686.6798 ns** |
| BooksJoinAuthors          | Neste(...)ndex) [24] |    50,509,812.500 ns |    679,954.7095 ns |    530,863.9117 ns |
| AuthorsJoinBooksOrdered   | Neste(...)ndex) [24] |             1.200 ns |          0.0120 ns |          0.0112 ns |
| BooksJoinAuthorsOrdered   | Neste(...)ndex) [24] |    50,979,725.359 ns |  1,019,539.3407 ns |  1,133,215.2758 ns |
| AuthorsJoinBooksUnordered | Neste(...)ndex) [24] |    49,635,234.118 ns |    928,814.5493 ns |    953,824.1585 ns |
| BooksJoinAuthorsUnordered | Neste(...)ndex) [24] |             1.190 ns |          0.0041 ns |          0.0039 ns |
| **AuthorsJoinBooks**          | **Sort-Merge Join**      |    **53,767,633.103 ns** |  **1,056,560.5221 ns** |  **1,548,692.7140 ns** |
| BooksJoinAuthors          | Sort-Merge Join      |    53,948,711.667 ns |  1,035,617.7452 ns |  1,346,595.5941 ns |
| AuthorsJoinBooksOrdered   | Sort-Merge Join      |    49,977,840.000 ns |    969,496.6765 ns |  1,226,102.8757 ns |
| BooksJoinAuthorsOrdered   | Sort-Merge Join      |    49,662,582.162 ns |    984,864.0342 ns |  1,672,373.8743 ns |
| AuthorsJoinBooksUnordered | Sort-Merge Join      |    54,494,161.818 ns |  1,078,414.0811 ns |  1,324,389.3393 ns |
| BooksJoinAuthorsUnordered | Sort-Merge Join      |    55,454,762.286 ns |  1,108,389.8238 ns |  1,821,116.2661 ns |
