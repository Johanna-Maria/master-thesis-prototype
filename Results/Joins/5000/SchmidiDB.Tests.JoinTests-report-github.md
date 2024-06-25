```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                    | JoinOperator         | Mean               | Error             | StdDev            |
|-------------------------- |--------------------- |-------------------:|------------------:|------------------:|
| **AuthorsJoinBooks**          | **Hash Join**            |  **21,300,650.240 ns** |   **283,853.4422 ns** |   **237,030.4801 ns** |
| BooksJoinAuthors          | Hash Join            |  20,901,902.604 ns |   145,396.2584 ns |   113,515.8348 ns |
| AuthorsJoinBooksOrdered   | Hash Join            |  21,066,902.708 ns |   277,141.7824 ns |   259,238.5952 ns |
| BooksJoinAuthorsOrdered   | Hash Join            |  21,105,991.458 ns |   315,549.0525 ns |   295,164.7795 ns |
| AuthorsJoinBooksUnordered | Hash Join            |  21,263,185.208 ns |   218,436.8393 ns |   204,325.9551 ns |
| BooksJoinAuthorsUnordered | Hash Join            |  21,191,176.875 ns |   398,085.3636 ns |   372,369.2961 ns |
| **AuthorsJoinBooks**          | **Nested-Loop Join**     | **769,975,221.429 ns** | **2,810,359.6655 ns** | **2,491,310.6421 ns** |
| BooksJoinAuthors          | Nested-Loop Join     | 761,630,300.000 ns | 6,968,534.5739 ns | 6,518,371.5641 ns |
| AuthorsJoinBooksOrdered   | Nested-Loop Join     | 992,465,885.714 ns | 8,522,919.3467 ns | 7,555,346.0047 ns |
| BooksJoinAuthorsOrdered   | Nested-Loop Join     | 763,179,440.000 ns | 1,913,504.0135 ns | 1,789,892.8415 ns |
| AuthorsJoinBooksUnordered | Nested-Loop Join     | 780,986,608.333 ns | 3,014,460.6229 ns | 2,353,492.5716 ns |
| BooksJoinAuthorsUnordered | Nested-Loop Join     | 918,424,907.692 ns | 4,641,139.9346 ns | 3,875,562.0436 ns |
| **AuthorsJoinBooks**          | **Neste(...)ndex) [24]** |  **21,181,518.750 ns** |   **369,262.1038 ns** |   **345,408.0010 ns** |
| BooksJoinAuthors          | Neste(...)ndex) [24] |  22,092,547.083 ns |   368,895.7006 ns |   345,065.2672 ns |
| AuthorsJoinBooksOrdered   | Neste(...)ndex) [24] |           1.214 ns |         0.0111 ns |         0.0104 ns |
| BooksJoinAuthorsOrdered   | Neste(...)ndex) [24] |  21,570,619.167 ns |   417,906.9294 ns |   390,910.4011 ns |
| AuthorsJoinBooksUnordered | Neste(...)ndex) [24] |  21,429,514.167 ns |   371,141.9900 ns |   347,166.4477 ns |
| BooksJoinAuthorsUnordered | Neste(...)ndex) [24] |           1.208 ns |         0.0100 ns |         0.0089 ns |
| **AuthorsJoinBooks**          | **Sort-Merge Join**      |  **20,669,728.125 ns** |   **231,326.8369 ns** |   **205,065.2155 ns** |
| BooksJoinAuthors          | Sort-Merge Join      |  20,690,419.583 ns |   400,212.2933 ns |   374,358.8275 ns |
| AuthorsJoinBooksOrdered   | Sort-Merge Join      |  20,238,292.500 ns |   312,164.2700 ns |   291,998.6518 ns |
| BooksJoinAuthorsOrdered   | Sort-Merge Join      |  20,293,085.833 ns |   366,236.2006 ns |   342,577.5693 ns |
| AuthorsJoinBooksUnordered | Sort-Merge Join      |  20,630,185.625 ns |   202,938.0705 ns |   189,828.3970 ns |
| BooksJoinAuthorsUnordered | Sort-Merge Join      |  20,790,508.958 ns |   277,921.3727 ns |   259,967.8245 ns |
