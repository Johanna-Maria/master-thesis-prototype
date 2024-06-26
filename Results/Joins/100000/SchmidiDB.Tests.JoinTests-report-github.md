```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                    | JoinOperator         | Mean                     | Error                 | StdDev                |
|-------------------------- |--------------------- |-------------------------:|----------------------:|----------------------:|
| **AuthorsJoinBooks**          | **Hash Join**            |       **518,440,778.571 ns** |     **9,133,672.1066 ns** |     **8,096,762.4181 ns** |
| BooksJoinAuthors          | Hash Join            |       513,616,806.667 ns |     9,714,301.2864 ns |     9,086,763.4505 ns |
| AuthorsJoinBooksOrdered   | Hash Join            |       493,796,505.556 ns |     9,399,209.5820 ns |    10,057,048.9476 ns |
| BooksJoinAuthorsOrdered   | Hash Join            |       511,032,380.000 ns |     9,864,819.2727 ns |     9,227,558.0683 ns |
| AuthorsJoinBooksUnordered | Hash Join            |       522,380,420.000 ns |     9,844,457.6776 ns |     9,208,511.8195 ns |
| BooksJoinAuthorsUnordered | Hash Join            |       525,243,653.333 ns |    10,315,234.0425 ns |     9,648,876.3234 ns |
| **AuthorsJoinBooks**          | **Nested-Loop Join**     |   **383,987,770,914.286 ns** |   **393,175,877.6953 ns** |   **348,540,174.5402 ns** |
| BooksJoinAuthors          | Nested-Loop Join     |   360,198,650,278.571 ns |   462,414,164.5449 ns |   409,918,112.3855 ns |
| AuthorsJoinBooksOrdered   | Nested-Loop Join     | 1,244,260,446,846.667 ns | 1,584,707,090.7323 ns | 1,482,335,995.8935 ns |
| BooksJoinAuthorsOrdered   | Nested-Loop Join     |   360,708,345,850.000 ns |   240,500,164.6521 ns |   213,197,131.6657 ns |
| AuthorsJoinBooksUnordered | Nested-Loop Join     |   375,254,648,946.667 ns |   593,451,235.3874 ns |   555,114,653.7849 ns |
| BooksJoinAuthorsUnordered | Nested-Loop Join     |   916,697,988,113.333 ns | 1,378,601,895.1679 ns | 1,289,545,068.0858 ns |
| **AuthorsJoinBooks**          | **Neste(...)ndex) [24]** |       **477,103,869.231 ns** |     **6,472,154.7424 ns** |     **5,404,542.3353 ns** |
| BooksJoinAuthors          | Neste(...)ndex) [24] |       484,733,913.333 ns |     8,844,787.4997 ns |     8,273,419.7150 ns |
| AuthorsJoinBooksOrdered   | Neste(...)ndex) [24] |                 1.310 ns |             0.0051 ns |             0.0048 ns |
| BooksJoinAuthorsOrdered   | Neste(...)ndex) [24] |       468,188,960.000 ns |     8,565,973.7713 ns |     8,012,617.1804 ns |
| AuthorsJoinBooksUnordered | Neste(...)ndex) [24] |       492,864,514.286 ns |     9,626,637.0136 ns |     8,533,762.9679 ns |
| BooksJoinAuthorsUnordered | Neste(...)ndex) [24] |                 1.344 ns |             0.0057 ns |             0.0053 ns |
| **AuthorsJoinBooks**          | **Sort-Merge Join**      |       **514,282,525.000 ns** |    **10,113,539.0715 ns** |     **9,932,847.6690 ns** |
| BooksJoinAuthors          | Sort-Merge Join      |       517,738,186.667 ns |     9,880,217.7387 ns |     9,241,961.8029 ns |
| AuthorsJoinBooksOrdered   | Sort-Merge Join      |       475,249,575.000 ns |     9,003,970.5277 ns |    10,368,985.8494 ns |
| BooksJoinAuthorsOrdered   | Sort-Merge Join      |       474,560,664.286 ns |     6,757,857.3019 ns |     5,990,664.4765 ns |
| AuthorsJoinBooksUnordered | Sort-Merge Join      |       534,618,073.333 ns |     7,152,239.5268 ns |     6,690,209.2909 ns |
| BooksJoinAuthorsUnordered | Sort-Merge Join      |       535,929,606.667 ns |     9,781,551.8745 ns |     9,149,669.6924 ns |
