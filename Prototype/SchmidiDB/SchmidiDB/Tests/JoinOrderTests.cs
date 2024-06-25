using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using OneOf;
using SchmidiDB.Query.Join;
using SchmidiDB.Query.Join.FromClause;
using SchmidiDB.Query.Join.Tree;
using SchmidiDB.Query.Selection;
using SchmidiDB.Query.Selection.Leaves;
using SchmidiDB.Storage;
using SchmidiDB.TestDatabases;
using SchmidiDB.TestDatabases.BookDBs;
using SchmidiDB.Tests.Util;

namespace SchmidiDB.Tests;

[SimpleJob(RuntimeMoniker.Net80)]
[RPlotExporter]
public class JoinOrderTests
{
    
    [ParamsSource(nameof(AllJoinTrees))]
    public JoinTreeWrapper JoinTreeMeasure;
    
    
    public IEnumerable<JoinTreeWrapper> AllJoinTrees
    {
        get { TestDatabase.UseTestDatabase(BookDatabases.BOOKS_100000);
            SystemCatalog systemCatalog = SystemCatalog.Instance;
            systemCatalog.Analyze();
            systemCatalog.CreateIndex("category", "books");
            systemCatalog.CreateIndex("author", "books");
            systemCatalog.CreateIndex("continent", "nationalities");
            systemCatalog.CreateIndex("nationality", "authors");
    //         var query = new Query.Query
    //         {
    //             From = new FromClause(
    //                 new Dictionary<string, OneOf<Table, Query.Query>>
    //                 {
    //                     ["b"] = systemCatalog.Tables["books"],
    //                     ["a"] = systemCatalog.Tables["authors"],
    //                     ["c"] = systemCatalog.Tables["categories"],
    //                     ["n"] = systemCatalog.Tables["nationalities"],
    //                     ["co"] = systemCatalog.Tables["continents"]
    //                 }, [
    //                     new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
    //                     new JoinDeclaration("b", "c", "category", "id", JoinType.InnerJoin),
    //                     new JoinDeclaration("co", "n", "id", "continent", JoinType.InnerJoin),
    //                     new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin)
    //                 ]
    //         
    //             )
    //         };
    
    var query = new Query.Query
    {
        From = new FromClause(
            new Dictionary<string, OneOf<Table, Query.Query>>
            {
                ["b"] = systemCatalog.Tables["books"],
                ["a"] = systemCatalog.Tables["authors"],
                ["c"] = systemCatalog.Tables["categories"],
                ["n"] = systemCatalog.Tables["nationalities"],
                ["co"] = systemCatalog.Tables["continents"]
            }, [
                new JoinDeclaration("b", "c", "category", "id", JoinType.OuterJoin),
                new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
                new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin),
                new JoinDeclaration("co", "n", "id", "continent", JoinType.OuterJoin)
            ]
    
        )
    };
    //
    //         // var query = new Query.Query
    //         // {
    //         //     From = new FromClause(
    //         //         new Dictionary<string, OneOf<Table,Query.Query>>
    //         //         {
    //         //             ["b"] = systemCatalog.Tables["books"],
    //         //             ["a"] = systemCatalog.Tables["authors"],
    //         //             ["c"] = systemCatalog.Tables["categories"],
    //         //             ["n"] = systemCatalog.Tables["nationalities"],
    //         //             ["co"] = systemCatalog.Tables["continents"]
    //         //         }, [
    //         //             new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
    //         //             new JoinDeclaration("b", "c", "category", "id", JoinType.InnerJoin),
    //         //             new JoinDeclaration("co", "n", "id", "continent", JoinType.InnerJoin),
    //         //             new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin)
    //         //         ]
    //         //
    //         //     ),
    //         //     Selection = new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //         new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //             new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //                 new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //                     new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //                         new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //                             new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //                                 new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //                                     new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
    //         //                                         new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")),
    //         //                                             new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist"))))))))))))
    //             // new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("Does Not Exist"))
    //             // Selection = new EqualitySelectionOperator(new SelectionLeaveColumn(""))
    //             // Selection = new AndSelectionOperator(new EqualitySelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Pamela")),
    //             //     new EqualitySelectionOperator(new SelectionLeaveColumn("a.gender"), new SelectionLeaveConstant("male"))) //this works 
    //             // Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.id"), new SelectionLeaveConstant(42))
    //
    //             // Selection = new AndSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"),
    //             //     new SelectionLeaveConstant("Surviving Amazing Leaves")), new EqualitySelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Mary")))
    //         // };
    //         
        // var query = new Query.Query
        // {
        // From = new FromClause(
        //     new Dictionary<string, OneOf<Table, Query.Query>>
        //     {
        //         ["b"] = new Query.Query
        //             {
        //                 From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
        //                 {
        //                     ["books"] = systemCatalog.Tables["books"]
        //                 }, [])
        //             },
        //         ["a"] = new Query.Query
        //         {
        //             From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
        //             {
        //                 ["authors"] = systemCatalog.Tables["authors"]
        //             }, [])
        //         },
        //         ["c"] = new Query.Query
        //         {
        //             From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
        //             {
        //                 ["categories"] = systemCatalog.Tables["categories"]
        //             }, [])
        //         },
        //         ["n"] = new Query.Query
        //         {
        //             From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
        //             {
        //                 ["nationalities"] = systemCatalog.Tables["nationalities"]
        //             }, [])
        //         },
        //         ["co"] = new Query.Query
        //         {
        //             From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
        //             {
        //                 ["continents"] = systemCatalog.Tables["continents"]
        //             }, [])
        //         },
        //     }, [
        //         new JoinDeclaration("b", "a", "books.author", "authors.id", JoinType.InnerJoin),
        //         new JoinDeclaration("c", "b", "categories.id", "books.category", JoinType.InnerJoin),
        //         new JoinDeclaration("n", "a", "nationalities.id", "authors.nationality", JoinType.InnerJoin),
        //         new JoinDeclaration("co", "n", "continents.id", "nationalities.continent", JoinType.InnerJoin),
        //
        //     ]
        // )
        
        // };
        
        
        query.PickQueryPlan();
        query.From.ExecutePreSelections();
        return query.AllJoinTreeResults.Select(pairpair => new JoinTreeWrapper(pairpair.Value.Value, pairpair.Value.Key, pairpair.Key)); }
    }
    
    // [IterationSetup]
    // public void IterationSetup()
    //     => Console.WriteLine($"Tree: {JoinTreeMeasure.JoinTree}, Estimated Rows: {JoinTreeMeasure.Rows}, Estimated Costs: {JoinTreeMeasure.Costs}");

    [Benchmark]
    public void ExecuteJoinTree()
    {
        var result = JoinTreeMeasure.JoinTree.Execute().ToList();
    }

    // public JoinTree jt { get; set; }
    // [GlobalSetup]
    // public void Setup()
    // {
    //     TestDatabase.UseTestDatabase(BookDatabases.BOOKS_1000000);
    //     SystemCatalog systemCatalog = SystemCatalog.Instance;
    //     systemCatalog.Analyze();
    //     systemCatalog.CreateIndex("category", "books");
    //     systemCatalog.CreateIndex("author", "books");
    //     systemCatalog.CreateIndex("continent", "nationalities");
    //     systemCatalog.CreateIndex("nationality", "authors");
    //     var from = new FromClause(
    //         new Dictionary<string, OneOf<Table, Query.Query>>
    //         {
    //             ["b"] = systemCatalog.Tables["books"],
    //             ["a"] = systemCatalog.Tables["authors"],
    //             ["c"] = systemCatalog.Tables["categories"],
    //             ["n"] = systemCatalog.Tables["nationalities"],
    //             ["co"] = systemCatalog.Tables["continents"]
    //         }, [
    //             new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
    //             new JoinDeclaration("b", "c", "category", "id", JoinType.InnerJoin),
    //             new JoinDeclaration("co", "n", "id", "continent", JoinType.InnerJoin),
    //             new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin)
    //         ]
    //
    //     );
    //     var con = new JoinTree(from, systemCatalog.Tables["continents"], systemCatalog.Tables["nationalities"],
    //         JoinType.InnerJoin, "id", "continent", new NestedLoopJoinWithIndexOperator(), "co", "n", [], []);
    //     var na = new JoinTree(from, con, systemCatalog.Tables["authors"],
    //         JoinType.InnerJoin, "n.id", "nationality", new HashJoinOperator(), null, "a", [], []);
    //     var ab =  new JoinTree(from, na, systemCatalog.Tables["books"],
    //         JoinType.InnerJoin, "a.id", "author", new NestedLoopJoinWithIndexOperator(), null, "b", [], []);
    //     jt = new JoinTree(from, ab, systemCatalog.Tables["categories"],
    //         JoinType.InnerJoin, "b.category", "id", new NestedLoopJoinWithIndexOperator(), null, "c", [], []);
    // }
    // [Benchmark]
    // public void MostEfficientJoinOrder()
    // {
    //     var res = jt.Execute().ToList();
    //
    // }
    

}