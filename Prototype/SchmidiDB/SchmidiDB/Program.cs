// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.Parameters;
using OneOf;
using SchmidiDB.Query;
using SchmidiDB.Query.Access;
using SchmidiDB.Query.Join;
using SchmidiDB.Query.Join.FromClause;
using SchmidiDB.Query.Join.Tree;
using SchmidiDB.Query.Selection;
using SchmidiDB.Query.Selection.Leaves;
using SchmidiDB.Storage;
using SchmidiDB.TestDatabases;
using OneOf;
using SchmidiDB.TestDatabases.BookDBs;
using SchmidiDB.Tests;


TestDatabase.UseTestDatabase(BookDatabases.BOOKS_100000);
SystemCatalog systemCatalog = SystemCatalog.Instance;
systemCatalog.Analyze();
systemCatalog.CreateIndex("category", "books");
systemCatalog.CreateIndex("author", "books");
systemCatalog.CreateIndex("continent", "nationalities");
systemCatalog.CreateIndex("nationality", "authors");
var query = new Query
{
    
    From = new FromClause(
        new Dictionary<string, OneOf<Table,Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["a"] = systemCatalog.Tables["authors"],
            // ["c"] = systemCatalog.Tables["categories"],
            // ["n"] = systemCatalog.Tables["nationalities"],
            // ["co"] = systemCatalog.Tables["continents"]
        }, [
            // new JoinDeclaration("b", "c", "category", "id", JoinType.OuterJoin),
            new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
            // new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin),
            // new JoinDeclaration("co", "n", "id", "continent", JoinType.OuterJoin)
        ]
    ),
    

};
var result = query.Execute().ToList();
Console.WriteLine(result.Count);
Console.WriteLine(result.First());

// Process process = Process.GetCurrentProcess();
// process.PriorityClass = ProcessPriorityClass.RealTime;
// BenchmarkRunner.Run<JoinTests>();
// BenchmarkRunner.Run<JoinTestsShort>();
// BenchmarkRunner.Run<JoinOrderTests>();
// BenchmarkRunner.Run<AccessBenchmark>();
