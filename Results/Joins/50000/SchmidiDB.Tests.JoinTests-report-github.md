```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-10700K CPU 3.80GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2

Job=.NET 8.0  Runtime=.NET 8.0  

```
| Method                    | JoinOperator         | Mean                   | Error                 | StdDev                |
|-------------------------- |--------------------- |-----------------------:|----------------------:|----------------------:|
| **AuthorsJoinBooks**          | **Hash Join**            |     **272,531,624.444 ns** |     **3,127,166.2048 ns** |     **2,925,153.2082 ns** |
| BooksJoinAuthors          | Hash Join            |     261,582,496.078 ns |     4,977,099.7379 ns |     5,111,114.9936 ns |
| AuthorsJoinBooksOrdered   | Hash Join            |     257,553,402.381 ns |     3,957,287.3320 ns |     3,508,032.1445 ns |
| BooksJoinAuthorsOrdered   | Hash Join            |     263,968,708.889 ns |     4,996,821.7823 ns |     4,674,030.1954 ns |
| AuthorsJoinBooksUnordered | Hash Join            |     273,201,655.556 ns |     4,540,463.2113 ns |     4,247,152.1050 ns |
| BooksJoinAuthorsUnordered | Hash Join            |     268,496,724.731 ns |     5,300,773.7144 ns |     8,094,841.3167 ns |
| **AuthorsJoinBooks**          | **Nested-Loop Join**     |  **97,161,837,364.286 ns** |   **181,543,110.7643 ns** |   **160,933,239.0462 ns** |
| BooksJoinAuthors          | Nested-Loop Join     |  81,043,487,915.385 ns |   734,781,146.1685 ns |   613,575,535.4570 ns |
| AuthorsJoinBooksOrdered   | Nested-Loop Join     | 253,151,084,350.000 ns |   560,298,046.6752 ns |   437,443,860.0056 ns |
| BooksJoinAuthorsOrdered   | Nested-Loop Join     |  80,937,549,933.333 ns |   371,450,761.7288 ns |   347,455,272.9856 ns |
| AuthorsJoinBooksUnordered | Nested-Loop Join     |  91,651,354,446.667 ns |   159,526,562.4645 ns |   149,221,245.5066 ns |
| BooksJoinAuthorsUnordered | Nested-Loop Join     | 126,184,501,283.333 ns | 2,389,811,021.1427 ns | 2,557,071,018.0923 ns |
| **AuthorsJoinBooks**          | **Neste(...)ndex) [24]** |     **232,500,257.778 ns** |     **3,587,768.5388 ns** |     **3,356,000.9172 ns** |
| BooksJoinAuthors          | Neste(...)ndex) [24] |     242,714,550.000 ns |     4,836,780.7221 ns |     7,810,499.2877 ns |
| AuthorsJoinBooksOrdered   | Neste(...)ndex) [24] |               1.179 ns |             0.0135 ns |             0.0126 ns |
| BooksJoinAuthorsOrdered   | Neste(...)ndex) [24] |     228,125,706.667 ns |     2,680,435.1658 ns |     2,507,280.7171 ns |
| AuthorsJoinBooksUnordered | Neste(...)ndex) [24] |     238,549,222.222 ns |     4,026,578.3608 ns |     3,766,463.8970 ns |
| BooksJoinAuthorsUnordered | Neste(...)ndex) [24] |               1.170 ns |             0.0110 ns |             0.0098 ns |
| **AuthorsJoinBooks**          | **Sort-Merge Join**      |     **246,782,323.333 ns** |     **4,716,263.9381 ns** |     **5,431,256.5646 ns** |
| BooksJoinAuthors          | Sort-Merge Join      |     245,308,282.051 ns |     3,323,464.3987 ns |     2,775,243.2934 ns |
| AuthorsJoinBooksOrdered   | Sort-Merge Join      |     230,607,638.095 ns |     4,398,034.5691 ns |     3,898,743.0901 ns |
| BooksJoinAuthorsOrdered   | Sort-Merge Join      |     229,856,964.583 ns |     4,341,196.6782 ns |     4,263,635.6078 ns |
| AuthorsJoinBooksUnordered | Sort-Merge Join      |     240,897,506.250 ns |     4,800,840.8167 ns |     4,715,067.6117 ns |
| BooksJoinAuthorsUnordered | Sort-Merge Join      |     242,459,440.000 ns |     4,683,967.7130 ns |     5,394,064.1837 ns |
