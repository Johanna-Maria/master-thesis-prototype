using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
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
using SchmidiDB.Tests.Util;
using System.Globalization;

namespace SchmidiDB.Tests;

public class JoinOrderTestsEstimation
{
    public void PrintAllInnerJoin()
    {
        var allDBs = new List<string>
        {
            BookDatabases.BOOKS_1000, BookDatabases.BOOKS_5000, BookDatabases.BOOKS_10000, BookDatabases.BOOKS_50000,
            BookDatabases.BOOKS_100000, BookDatabases.BOOKS_500000, BookDatabases.BOOKS_1000000
        };
        var results = new Dictionary<string, List<JoinTreeResult>>
        {
            ["co, n, a, b, c"] = [],
            ["n, a, co, b, c"] = [],
            ["n, a, b, c, co"] = [],
            ["n, a, b, co, c"] = [],
            ["b, c, a, n, co"] = [],
            ["b, a, n, c, co"] = [],
            ["b, a, n, co, c"] = [],
            ["b, a, c, n, co"] = [],

        };
        foreach (string db in allDBs)
        {
            TestDatabase.UseTestDatabase(db);
            SystemCatalog systemCatalog = SystemCatalog.Instance;
            systemCatalog.Analyze();
            systemCatalog.CreateIndex("category", "books");
            systemCatalog.CreateIndex("author", "books");
            systemCatalog.CreateIndex("continent", "nationalities");
            systemCatalog.CreateIndex("nationality", "authors");
            var query = new Query.Query
            {
                From = new FromClause(
                    new Dictionary<string, OneOf<Table,Query.Query>>
                    {
                        ["b"] = systemCatalog.Tables["books"],
                        ["a"] = systemCatalog.Tables["authors"],
                        ["c"] = systemCatalog.Tables["categories"],
                        ["n"] = systemCatalog.Tables["nationalities"],
                        ["co"] = systemCatalog.Tables["continents"]
                    }, [
                        new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
                        new JoinDeclaration("b", "c", "category", "id", JoinType.InnerJoin),
                        new JoinDeclaration("co", "n", "id", "continent", JoinType.InnerJoin),
                        new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin)
                    ]
            
                )
            };
            query.PickQueryPlan();
            var partialResult = query.PrintAndExecuteAllJoinTrees(false);
            foreach (var res in partialResult)
            {
                results[res.JoinShort] = results[res.JoinShort].Append(res).ToList();
            }
            SystemCatalog.Delete();
        }

        foreach (var pair in results)
        {
            Console.WriteLine(pair.Key);
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Costs.ToString(CultureInfo.InvariantCulture)))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => "'"+res.JoinOperators+"'"))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Rows.ToString(CultureInfo.InvariantCulture)))}]");

        }
    }

    public void PrintOuterJoin()
    {
        var allDBs = new List<string>
        {
            BookDatabases.BOOKS_100000
        };
        var results = new Dictionary<string, List<JoinTreeResult>>
        {
            ["n, a, b, c, co"] = [],
            ["b, a, n, c, co"] = [],

        };
        foreach (string db in allDBs)
        {
            TestDatabase.UseTestDatabase(db);
            SystemCatalog systemCatalog = SystemCatalog.Instance;
            systemCatalog.Analyze();
            systemCatalog.CreateIndex("category", "books");
            systemCatalog.CreateIndex("author", "books");
            systemCatalog.CreateIndex("continent", "nationalities");
            systemCatalog.CreateIndex("nationality", "authors");
            var query = new Query.Query
            {
                From = new FromClause(
                    new Dictionary<string, OneOf<Table,Query.Query>>
                    {
                        ["b"] = systemCatalog.Tables["books"],
                        ["a"] = systemCatalog.Tables["authors"],
                        ["c"] = systemCatalog.Tables["categories"],
                        ["n"] = systemCatalog.Tables["nationalities"],
                        ["co"] = systemCatalog.Tables["continents"]
                    }, [
                        new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
                        new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin),
                        new JoinDeclaration("b", "c", "category", "id", JoinType.OuterJoin),
                        new JoinDeclaration("co", "n", "id", "continent", JoinType.OuterJoin)
                    ]
            
                )
            };
            query.PickQueryPlan();
            var partialResult = query.PrintAndExecuteAllJoinTrees(false);
            foreach (var res in partialResult)
            {
                results[res.JoinShort] = results[res.JoinShort].Append(res).ToList();
            }
            SystemCatalog.Delete();
        }

        foreach (var pair in results)
        {
            Console.WriteLine(pair.Key);
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Costs.ToString(CultureInfo.InvariantCulture)))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => "'"+res.JoinOperators+"'"))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Rows.ToString(CultureInfo.InvariantCulture)))}]");

        }
    }
    
    public void PrintOuterJoin2()
    {
        var allDBs = new List<string>
        {
            BookDatabases.BOOKS_100000
        };
        var results = new Dictionary<string, List<JoinTreeResult>>
        {
            ["b, c, a, n, co"] = [],

        };
        foreach (string db in allDBs)
        {
            TestDatabase.UseTestDatabase(db);
            SystemCatalog systemCatalog = SystemCatalog.Instance;
            systemCatalog.Analyze();
            systemCatalog.CreateIndex("category", "books");
            systemCatalog.CreateIndex("author", "books");
            systemCatalog.CreateIndex("continent", "nationalities");
            systemCatalog.CreateIndex("nationality", "authors");
            var query = new Query.Query
            {
                From = new FromClause(
                    new Dictionary<string, OneOf<Table,Query.Query>>
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
            query.PickQueryPlan();
            var partialResult = query.PrintAndExecuteAllJoinTrees(false);
            foreach (var res in partialResult)
            {
                results[res.JoinShort] = results[res.JoinShort].Append(res).ToList();
            }
            SystemCatalog.Delete();
        }

        foreach (var pair in results)
        {
            Console.WriteLine(pair.Key);
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Costs.ToString(CultureInfo.InvariantCulture)))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => "'"+res.JoinOperators+"'"))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Rows.ToString(CultureInfo.InvariantCulture)))}]");

        }
    }
    
    public void PrintSubQueries()
    {
        var allDBs = new List<string>
        {
            BookDatabases.BOOKS_100000
        };
        var results = new Dictionary<string, List<JoinTreeResult>>
        {
            ["co, n, a, b, c"] = [],
            ["n, a, co, b, c"] = [],
            ["n, a, b, c, co"] = [],
            ["n, a, b, co, c"] = [],
            ["b, c, a, n, co"] = [],
            ["b, a, n, c, co"] = [],
            ["b, a, n, co, c"] = [],
            ["b, a, c, n, co"] = [],
            ["c, b, a, n, co"] = []

        };
        foreach (string db in allDBs)
        {
            TestDatabase.UseTestDatabase(db);
            SystemCatalog systemCatalog = SystemCatalog.Instance;
            systemCatalog.Analyze();
            systemCatalog.CreateIndex("category", "books");
            systemCatalog.CreateIndex("author", "books");
            systemCatalog.CreateIndex("continent", "nationalities");
            systemCatalog.CreateIndex("nationality", "authors");
        var query = new Query.Query
        {
        From = new FromClause(
            new Dictionary<string, OneOf<Table, Query.Query>>
            {
                ["b"] = new Query.Query
                    {
                        From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
                        {
                            ["books"] = systemCatalog.Tables["books"]
                        }, [])
                    },
                ["a"] = new Query.Query
                {
                    From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
                    {
                        ["authors"] = systemCatalog.Tables["authors"]
                    }, [])
                },
                ["c"] = new Query.Query
                {
                    From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
                    {
                        ["categories"] = systemCatalog.Tables["categories"]
                    }, [])
                },
                ["n"] = new Query.Query
                {
                    From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
                    {
                        ["nationalities"] = systemCatalog.Tables["nationalities"]
                    }, [])
                },
                ["co"] = new Query.Query
                {
                    From = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
                    {
                        ["continents"] = systemCatalog.Tables["continents"]
                    }, [])
                },
            }, [
                new JoinDeclaration("b", "a", "books.author", "authors.id", JoinType.InnerJoin),
                new JoinDeclaration("c", "b", "categories.id", "books.category", JoinType.InnerJoin),
                new JoinDeclaration("n", "a", "nationalities.id", "authors.nationality", JoinType.InnerJoin),
                new JoinDeclaration("co", "n", "continents.id", "nationalities.continent", JoinType.InnerJoin),
    
            ]
        )
        
    };
            query.PickQueryPlan();
            var partialResult = query.PrintAndExecuteAllJoinTrees(false);
            foreach (var res in partialResult)
            {
                results[res.JoinShort] = results[res.JoinShort].Append(res).ToList();
            }
            SystemCatalog.Delete();
        }

        foreach (var pair in results)
        {
            Console.WriteLine(pair.Key);
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Costs.ToString(CultureInfo.InvariantCulture)))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => "'"+res.JoinOperators+"'"))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Rows.ToString(CultureInfo.InvariantCulture)))}]");

        }
    }
    
    
    public void PrintFaulty()
    {
        var allDBs = new List<string>
        {
            BookDatabases.BOOKS_100000
        };
        var results = new Dictionary<string, List<JoinTreeResult>>
        {
            ["co, n, a, b, c"] = [],
            ["n, a, co, b, c"] = [],
            ["n, a, b, c, co"] = [],
            ["n, a, b, co, c"] = [],
            ["b, c, a, n, co"] = [],
            ["b, a, n, c, co"] = [],
            ["b, a, n, co, c"] = [],
            ["b, a, c, n, co"] = [],

        };
        foreach (string db in allDBs)
        {
            TestDatabase.UseTestDatabase(db);
            SystemCatalog systemCatalog = SystemCatalog.Instance;
            systemCatalog.Analyze();
            systemCatalog.CreateIndex("category", "books");
            systemCatalog.CreateIndex("author", "books");
            systemCatalog.CreateIndex("continent", "nationality");
            systemCatalog.CreateIndex("nationality", "authors");
            var query = new Query.Query
            {
                From = new FromClause(
                    new Dictionary<string, OneOf<Table,Query.Query>>
                    {
                        ["b"] = systemCatalog.Tables["books"],
                        ["a"] = systemCatalog.Tables["authors"],
                        ["c"] = systemCatalog.Tables["categories"],
                        ["n"] = systemCatalog.Tables["nationalities"],
                        ["co"] = systemCatalog.Tables["continents"]
                    }, [
                        new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
                        new JoinDeclaration("b", "c", "category", "id", JoinType.InnerJoin),
                        new JoinDeclaration("co", "n", "id", "continent", JoinType.InnerJoin),
                        new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin)
                    ]
            
                ),
                Selection = new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                    new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                        new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                            new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                                new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                                    new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                                        new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                                            new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                                                new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")), 
                                                    new OrSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist")),
                                                        new LikeSelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("does not exist"))))))))))))
            };
            query.PickQueryPlan();
            var partialResult = query.PrintAndExecuteAllJoinTrees(false);
            foreach (var res in partialResult)
            {
                results[res.JoinShort] = results[res.JoinShort].Append(res).ToList();
            }
            SystemCatalog.Delete();
        }

        foreach (var pair in results)
        {
            Console.WriteLine(pair.Key);
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Costs.ToString(CultureInfo.InvariantCulture)))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => "'"+res.JoinOperators+"'"))}]");
            Console.WriteLine($"[{String.Join(',', pair.Value.Select(res => res.Rows.ToString(CultureInfo.InvariantCulture)))}]");

        }
    }
}