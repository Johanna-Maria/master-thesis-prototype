using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SchmidiDB.Query.Join;
using SchmidiDB.Storage;
using SchmidiDB.TestDatabases;
using SchmidiDB.TestDatabases.BookDBs;

namespace SchmidiDB.Tests;

[SimpleJob(RuntimeMoniker.Net80)]
[RPlotExporter]
public class JoinTests
{
    private IEnumerable<Row> authors;
    private IEnumerable<Row> books;
    private IEnumerable<Row> authorsUnordered;
    private IEnumerable<Row> booksOrdered;

    
    [GlobalSetup]
    public void Setup()
    {
        TestDatabase.UseTestDatabase(BookDatabases.BOOKS_1000000);
        SystemCatalog systemCatalog = SystemCatalog.Instance;
        systemCatalog.Analyze();
        systemCatalog.CreateIndex("author", "books");
        authors = systemCatalog.Tables["authors"];
        books = systemCatalog.Tables["books"];
        authorsUnordered = authors.OrderBy(row => row["firstName"]).ToList();
        booksOrdered = books.OrderBy(row => row["author"]).ToList();
    }

    [ParamsSource(nameof(JoinOperators))]
    public IJoinOperator JoinOperator { get; set; }

    public IEnumerable<IJoinOperator> JoinOperators => new IJoinOperator[]
    {
        // new NestedLoopJoinOperator(),
        new NestedLoopJoinWithIndexOperator(),
        new SortMergeJoinOperator(),
        new HashJoinOperator()
    };
    
    // [ParamsSource(nameof(JoinOperatorsWithoutNLI))]
    // public IJoinOperator JoinOperatorWithoutNLI { get; set; }
    //
    // public IEnumerable<IJoinOperator> JoinOperatorsWithoutNLI => new IJoinOperator[]
    // {
    //     new SortMergeJoinOperator(),
    //     new HashJoinOperator()
    // };


    [Benchmark]
    public void AuthorsJoinBooks()
    {

        var res = JoinOperator.Join(authors, "authors", books, "books","id",
            "author", ["authors.id"], ["books.id"], [], []).ToList();


    }
    
    [Benchmark]
    public void BooksJoinAuthors()
    {

        var res = JoinOperator.Join(books, "books", authors, "authors","author",
            "id", ["books.id"], ["authors.id"], [], []).ToList();


    }
    
    [Benchmark]
    public void AuthorsJoinBooksOrdered()
    {
        if (JoinOperator is NestedLoopJoinWithIndexOperator) return;
        var res = JoinOperator.Join(authors, "authors", booksOrdered, "books","id",
                "author", ["authors.id"], ["books.author"],[], []).ToList();

    }
    
    [Benchmark]
    public void BooksJoinAuthorsOrdered()
    {
        
        var res = JoinOperator.Join(booksOrdered, "books", authors, "authors","author",
            "id", ["books.author"], ["authors.id"], [], []).ToList();

    }
    
    [Benchmark]
    public void AuthorsJoinBooksUnordered()
    {
        
        var res = JoinOperator.Join(authorsUnordered, "authors", books, "books","id",
            "author", ["authors.firstName"], ["books.id"], [], []).ToList();

    }
    
    [Benchmark]
    public void BooksJoinAuthorsUnordered()
    {
        if (JoinOperator is NestedLoopJoinWithIndexOperator) return;
        var res = JoinOperator.Join(books, "books", authorsUnordered, "authors","author",
            "id", ["books.id"], ["authors.firstName"], [], []).ToList();
        

    }

}