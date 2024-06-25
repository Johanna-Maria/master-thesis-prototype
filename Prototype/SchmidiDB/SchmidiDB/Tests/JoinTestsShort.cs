using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SchmidiDB.Query.Join;
using SchmidiDB.Storage;
using SchmidiDB.TestDatabases;

namespace SchmidiDB.Tests;

[SimpleJob(RuntimeMoniker.Net80)]
[RPlotExporter]
public class JoinTestsShort
{
    // private IEnumerable<Row> categories;
    // private IEnumerable<Row> books;
    private IEnumerable<Row> authors;
    private IEnumerable<Row> nationalities;
    private IEnumerable<Row> continents;
    private IEnumerable<Row> authorsUnordered;
    private IEnumerable<Row> booksOrdered;

    
    [GlobalSetup]
    public void Setup()
    {
        TestDatabase.UseTestDatabase();
        SystemCatalog systemCatalog = SystemCatalog.Instance;
        systemCatalog.Analyze();
        systemCatalog.CreateIndex("nationality", "authors");
        systemCatalog.CreateIndex("category", "books");
        systemCatalog.CreateIndex("continent", "nationalities");
        authors = systemCatalog.Tables["authors"];
        // books = systemCatalog.Tables["books"];
        nationalities = systemCatalog.Tables["nationalities"];
        continents = systemCatalog.Tables["continents"];
    }

    [ParamsSource(nameof(JoinOperators))]
    public IJoinOperator JoinOperator { get; set; }

    public IEnumerable<IJoinOperator> JoinOperators => new IJoinOperator[]
    {
        new NestedLoopJoinOperator(),
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
    public void AuthorsJoinNationalities()
    {

        var res = JoinOperator.Join(authors, "authors", nationalities, "nationalities","nationality",
            "id", ["authors.id"], ["nationalities.id"], [], []).ToList();


    }
    
    [Benchmark]
    public void NationalitiesJoinAuthors()
    {

        var res = JoinOperator.Join(nationalities, "nationalities", authors, "authors","id",
            "nationality", ["nationalities.id"], ["authors.id"], [], []).ToList();


    }
    
    [Benchmark]
    public void NationalitiesJoinContinents()
    {
        
        var res = JoinOperator.Join(nationalities, "nationalities", continents, "continents","continent",
            "id", ["nationalities.id"], ["continents.id"], [], []).ToList();

    }
    
    [Benchmark]
    public void ContinentsJoinNationalities()
    {
        var res = JoinOperator.Join(continents, "continents", nationalities, "nationalities","id",
            "continent", ["continents.id"], ["nationalities.id"], [], []).ToList();
        

    }

}