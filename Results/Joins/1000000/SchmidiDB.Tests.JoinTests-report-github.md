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
| **AuthorsJoinBooks**          | **Hash Join**            | **5,311,574,085.714 ns** | **40,159,768.3459 ns** | **35,600,588.5987 ns** |
| BooksJoinAuthors          | Hash Join            | 5,153,768,640.000 ns | 25,985,047.6472 ns | 24,306,429.6915 ns |
| AuthorsJoinBooksOrdered   | Hash Join            | 5,000,839,992.857 ns | 30,320,764.2668 ns | 26,878,567.7588 ns |
| BooksJoinAuthorsOrdered   | Hash Join            | 5,082,086,060.000 ns | 22,770,377.8630 ns | 21,299,425.5808 ns |
| AuthorsJoinBooksUnordered | Hash Join            | 5,231,530,300.000 ns | 48,030,666.6569 ns | 44,927,915.3912 ns |
| BooksJoinAuthorsUnordered | Hash Join            | 5,217,985,876.923 ns | 14,871,519.3927 ns | 12,418,392.2274 ns |
| **AuthorsJoinBooks**          | **Neste(...)ndex) [24]** | **4,808,891,407.143 ns** | **19,422,289.7398 ns** | **17,217,353.9627 ns** |
| BooksJoinAuthors          | Neste(...)ndex) [24] | 4,891,159,557.143 ns | 22,574,987.9280 ns | 20,012,138.7884 ns |
| AuthorsJoinBooksOrdered   | Neste(...)ndex) [24] |             1.286 ns |          0.0081 ns |          0.0072 ns |
| BooksJoinAuthorsOrdered   | Neste(...)ndex) [24] | 4,670,181,000.000 ns | 18,707,341.1275 ns | 16,583,570.6402 ns |
| AuthorsJoinBooksUnordered | Neste(...)ndex) [24] | 4,903,936,753.333 ns | 24,105,488.9295 ns | 22,548,289.3007 ns |
| BooksJoinAuthorsUnordered | Neste(...)ndex) [24] |             1.338 ns |          0.0065 ns |          0.0057 ns |
| **AuthorsJoinBooks**          | **Sort-Merge Join**      | **5,542,651,907.143 ns** | **20,593,967.4323 ns** | **18,256,015.7184 ns** |
| BooksJoinAuthors          | Sort-Merge Join      | 5,582,455,042.857 ns | 21,310,918.7771 ns | 18,891,574.4113 ns |
| AuthorsJoinBooksOrdered   | Sort-Merge Join      | 4,770,540,226.667 ns | 20,513,260.7998 ns | 19,188,116.8795 ns |
| BooksJoinAuthorsOrdered   | Sort-Merge Join      | 4,754,536,526.667 ns | 21,823,666.3695 ns | 20,413,871.0624 ns |
| AuthorsJoinBooksUnordered | Sort-Merge Join      | 5,959,464,860.000 ns | 31,259,357.7529 ns | 29,240,022.6367 ns |
| BooksJoinAuthorsUnordered | Sort-Merge Join      | 5,975,271,940.000 ns | 39,826,552.4439 ns | 37,253,781.8661 ns |
