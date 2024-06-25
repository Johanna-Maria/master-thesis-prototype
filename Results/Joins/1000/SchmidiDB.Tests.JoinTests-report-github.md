```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                    | JoinOperator         | Mean              | Error           | StdDev          |
|-------------------------- |--------------------- |------------------:|----------------:|----------------:|
| **AuthorsJoinBooks**          | **Hash Join**            |  **2,260,057.091 ns** |  **20,366.7930 ns** |  **17,007.1945 ns** |
| BooksJoinAuthors          | Hash Join            |  2,233,233.594 ns |  15,047.4185 ns |  14,075.3646 ns |
| AuthorsJoinBooksOrdered   | Hash Join            |  2,234,834.147 ns |  11,637.7851 ns |   9,086.0171 ns |
| BooksJoinAuthorsOrdered   | Hash Join            |  2,214,578.776 ns |  13,231.2324 ns |  12,376.5030 ns |
| AuthorsJoinBooksUnordered | Hash Join            |  2,249,508.984 ns |  18,113.6848 ns |  16,057.3098 ns |
| BooksJoinAuthorsUnordered | Hash Join            |  2,223,210.130 ns |  10,162.9069 ns |   9,506.3894 ns |
| **AuthorsJoinBooks**          | **Nested-Loop Join**     | **31,952,985.417 ns** | **107,296.3070 ns** | **100,365.0321 ns** |
| BooksJoinAuthors          | Nested-Loop Join     | 31,609,191.250 ns | 125,151.6738 ns | 117,066.9533 ns |
| AuthorsJoinBooksOrdered   | Nested-Loop Join     | 38,283,671.429 ns |  86,151.9983 ns |  76,371.5025 ns |
| BooksJoinAuthorsOrdered   | Nested-Loop Join     | 30,900,732.589 ns |  83,710.8536 ns |  74,207.4913 ns |
| AuthorsJoinBooksUnordered | Nested-Loop Join     | 31,965,666.827 ns |  73,134.7919 ns |  61,070.8635 ns |
| BooksJoinAuthorsUnordered | Nested-Loop Join     | 32,495,226.667 ns |  98,044.5461 ns |  91,710.9292 ns |
| **AuthorsJoinBooks**          | **Neste(...)ndex) [24]** |  **2,193,258.594 ns** |  **14,114.5920 ns** |  **15,688.3317 ns** |
| BooksJoinAuthors          | Neste(...)ndex) [24] |  2,211,065.458 ns |  13,572.6731 ns |  12,031.8212 ns |
| AuthorsJoinBooksOrdered   | Neste(...)ndex) [24] |          1.200 ns |       0.0137 ns |       0.0128 ns |
| BooksJoinAuthorsOrdered   | Neste(...)ndex) [24] |  2,192,387.005 ns |  10,206.0373 ns |   9,546.7337 ns |
| AuthorsJoinBooksUnordered | Neste(...)ndex) [24] |  2,141,927.567 ns |   6,911.8580 ns |   6,127.1821 ns |
| BooksJoinAuthorsUnordered | Neste(...)ndex) [24] |          1.200 ns |       0.0088 ns |       0.0078 ns |
| **AuthorsJoinBooks**          | **Sort-Merge Join**      |  **2,322,767.718 ns** |  **11,896.1511 ns** |  **10,545.6281 ns** |
| BooksJoinAuthors          | Sort-Merge Join      |  2,371,548.151 ns |  14,496.8079 ns |  13,560.3231 ns |
| AuthorsJoinBooksOrdered   | Sort-Merge Join      |  2,205,918.620 ns |  17,516.8648 ns |  16,385.2862 ns |
| BooksJoinAuthorsOrdered   | Sort-Merge Join      |  2,153,990.417 ns |  10,568.6983 ns |   9,885.9669 ns |
| AuthorsJoinBooksUnordered | Sort-Merge Join      |  2,389,671.354 ns |   7,924.8357 ns |   6,187.1904 ns |
| BooksJoinAuthorsUnordered | Sort-Merge Join      |  2,405,822.566 ns |   8,441.7613 ns |   7,049.2530 ns |
