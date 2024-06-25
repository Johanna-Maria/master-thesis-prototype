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

namespace SchmidiDB.Tests;

public class SelectionTests
{
    private SystemCatalog systemCatalog;
    
    public SelectionTests()
    {
        TestDatabase.UseTestDatabase();
        systemCatalog = SystemCatalog.Instance;
        systemCatalog.Analyze();
    }
    
    public void ExecuteSelectionTests()
    {
        // Console.WriteLine("SELECT * FROM authors a WHERE a.firstName = 'Pamela'");
        // SelectionSimple();
        // Console.WriteLine();
        // Console.WriteLine("SELECT * FROM books b WHERE b.title = 'Dancing Fish'");
        // SelectionCalculated2();
        // Console.WriteLine("SELECT * FROM books b WHERE b.title = 'Surviving Amazing Leaves'");
        // SelectionCalculated3();
        // Console.WriteLine("SELECT * FROM books b WHERE b.title = 'The Journey of Three Stars'");
        // SelectionCalculated1();
        // Console.WriteLine();
        // Console.WriteLine("SELECT * FROM authors a WHERE a.firstName = 'Pamela' AND a.lastName = 'Sullivan'");
        // SelectionIndependentAnd();
        // Console.WriteLine();
        // Console.WriteLine("SELECT * FROM authors a WHERE a.firstName = 'Pamela' OR a.lastName = 'Sullivan'");
        // SelectionIndependentOr();
        // Console.WriteLine();
        // Console.WriteLine("SELECT * FROM authors a WHERE a.firstName = 'Pamela' AND a.gender = 'female'");
        // SelectionDependent();
        // Console.WriteLine("SELECT * FROM books b WHERE b.title = 'The Journey of Three Stars' AND b.year = 2001");
        // SelectionDependentCalculated1();
        // Console.WriteLine("SELECT * FROM books b WHERE b.title = 'Dancing Fish' AND b.year = 2001");
        // SelectionDependentCalculated2();
        // Console.WriteLine("SELECT * FROM books b WHERE b.title = 'Surviving Amazing Leaves' AND b.year = 2001");
        // SelectionDependentCalculated3();
        Console.WriteLine("SELECT * FROM authors a WHERE a.firstName LIKE 'Pamela'");
        SelectionLike();
        // Console.WriteLine("SELECT * FROM books b WHERE b.category IS NULL");
        // IsNull();
    }

    public void ExecuteJoinSelectionTests()
    {
        Console.WriteLine("SELECT * FROM authors JOIN public.books b on authors.id = b.author;");
        // AuthorsInnerJoinBooks();
        Console.WriteLine("SELECT * FROM books JOIN public.categories c on c.id = books.category;");
        // BooksInnerJoinCategories();
        Console.WriteLine("SELECT * FROM nationalities JOIN public.continents c on c.id = nationalities.continent;");
        // NationalitiesInnerJoinContinents();
        Console.WriteLine("SELECT * FROM authors FULL OUTER JOIN public.books b on authors.id = b.author;");
        // AuthorsOuterJoinBooks();
        Console.WriteLine("SELECT * FROM books FULL OUTER JOIN public.categories c on c.id = books.category;");
        // BooksOuterJoinCategories();
        Console.WriteLine("SELECT * FROM books JOIN public.categories c on c.id = books.category WHERE books.category IS NOT NULL;");
        // BooksOuterJoinCategoriesIsNotNull();
        Console.WriteLine("SELECT * FROM nationalities FULL OUTER JOIN public.continents c on c.id = nationalities.continent;");
        // NationalitiesOuterJoinContinents();
        Console.WriteLine("SELECT * FROM books JOIN public.categories c on (c.id = books.category) JOIN public.authors a on (a.id = books.author) JOIN nationalities  n on (a.nationality = n.id) JOIN continents  c2 on n.continent = c2.id;");
        // AllInnerJoin();
        Console.WriteLine("SELECT * FROM books FULL OUTER JOIN public.categories c on (c.id = books.category) FULL OUTER JOIN public.authors a on (a.id = books.author) INNER JOIN nationalities  n on (a.nationality = n.id) FULL OUTER JOIN continents  c2 on n.continent = c2.id;");
        AllTuplesJoin();
        Console.WriteLine("SELECT * FROM books INNER JOIN public.authors a on a.id = books.author WHERE firstname = 'Pamela' AND year = '2001';");
        // Pamela2001();
        Console.WriteLine("SELECT * FROM books INNER JOIN public.authors a on a.id = books.author WHERE firstname LIKE 'Pamela' AND year = '2001';");
        // LikePamela2001();
        // SubQueries1();
        // SubQueries2();
    }
    
    void SelectionSimple()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table,Query.Query>>
        {
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, [
            ]
        );

        query.Selection =
            new EqualitySelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Pamela"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionIndependentAnd()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new AndSelectionOperator(new EqualitySelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Pamela")),
            new EqualitySelectionOperator(new SelectionLeaveColumn("a.lastName"), new SelectionLeaveConstant("Sullivan")));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionIndependentOr()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new OrSelectionOperator(new EqualitySelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Pamela")),
            new EqualitySelectionOperator(new SelectionLeaveColumn("a.lastName"), new SelectionLeaveConstant("Sullivan")));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionDependent()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new AndSelectionOperator(new EqualitySelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Pamela")),
            new EqualitySelectionOperator(new SelectionLeaveColumn("a.gender"), new SelectionLeaveConstant("female")));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionCalculated1()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        // query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("Dancing Fish"));
        query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("The Journey of Three Stars"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionCalculated2()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("Dancing Fish"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionCalculated3()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("Surviving Amazing Leaves"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionDependentCalculated1()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new OrSelectionOperator(new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("The Journey of Three Stars")),
            new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001)));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionDependentCalculated2()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new OrSelectionOperator(new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("Dancing Fish")),
            new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001)));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionDependentCalculated3()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        query.Selection = new OrSelectionOperator(new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"), new SelectionLeaveConstant("Surviving Amazing Leaves")),
            new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001)));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SelectionLike()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, [
            ]
        );
        
        // query.Selection = new AndSelectionOperator(new LikeSelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Pamela")),
        //     new LikeSelectionOperator(new SelectionLeaveColumn("a.lastName"), new SelectionLeaveConstant("Sullivan")));
        query.Selection = new LikeSelectionOperator(new SelectionLeaveColumn("a.firstName"),
            new SelectionLeaveConstant("Pamela"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void IsNull()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"]
        };
        query.From = new FromClause( tmp, [
            ]
        );

        query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void AuthorsInnerJoinBooks()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("a", "b", "id", "author", JoinType.InnerJoin)
            ]
        );

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void AuthorsOuterJoinBooks()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("a", "b", "id", "author", JoinType.OuterJoin)
            ]
        );

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void BooksInnerJoinCategories()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["c"] = systemCatalog.Tables["categories"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("b", "c", "category", "id", JoinType.InnerJoin)
            ]
        );

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void BooksOuterJoinCategories()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["c"] = systemCatalog.Tables["categories"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("b", "c", "category", "id", JoinType.OuterJoin)
            ]
        );

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void BooksOuterJoinCategoriesIsNotNull()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["c"] = systemCatalog.Tables["categories"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("b", "c", "category", "id", JoinType.InnerJoin)
            ]
        );

        query.Selection = new IsNotNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void NationalitiesInnerJoinContinents()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["n"] = systemCatalog.Tables["nationalities"],
            ["co"] = systemCatalog.Tables["continents"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("n", "co", "continent", "id", JoinType.InnerJoin)
            ]
        );

        // query.Selection = new IsNotNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void NationalitiesOuterJoinContinents()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["n"] = systemCatalog.Tables["nationalities"],
            ["co"] = systemCatalog.Tables["continents"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("n", "co", "continent", "id", JoinType.OuterJoin)
            ]
        );

        // query.Selection = new IsNotNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void AllInnerJoin()
    {
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
                    new JoinDeclaration("b", "a", "author", "id", JoinType.InnerJoin),
                    new JoinDeclaration("c", "b", "id", "category", JoinType.InnerJoin),
                    new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin),
                    new JoinDeclaration("co", "n", "id", "continent", JoinType.InnerJoin),

                ]
            )
            
        };
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void AllTuplesJoin()
    {
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
                    new JoinDeclaration("c", "b", "id", "category", JoinType.OuterJoin),
                    new JoinDeclaration("b", "a", "author", "id", JoinType.OuterJoin),
                    new JoinDeclaration("n", "a", "id", "nationality", JoinType.InnerJoin),
                    new JoinDeclaration("co", "n", "id", "continent", JoinType.OuterJoin),

                ]
            )
            
        };
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void Pamela2001()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("a", "b", "id", "author", JoinType.InnerJoin)
            ]
        );
        query.Selection =
            new AndSelectionOperator(
                new EqualitySelectionOperator(new SelectionLeaveColumn("a.firstName"),
                    new SelectionLeaveConstant("Pamela")), new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001)));

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void LikePamela2001()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, 
            [
                new JoinDeclaration("a", "b", "id", "author", JoinType.InnerJoin)
            ]
        );
        query.Selection =
            new AndSelectionOperator(
                new LikeSelectionOperator(new SelectionLeaveColumn("a.firstName"), new SelectionLeaveConstant("Pamela")), new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001)));

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SubQueries1()
    {
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
                }, [
                    new JoinDeclaration("b", "a", "books.author", "authors.id", JoinType.InnerJoin),

                ]
            )
            
        };
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void SubQueries2()
    {
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
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }

    void JoinLikeBooksQuery()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            // ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, 
            [
                // new JoinDeclaration("a", "b", "id", "author", JoinType.InnerJoin)
            ]
        );
        query.Selection = new LikeSelectionOperator(new SelectionLeaveColumn("b.title"),
            new SelectionLeaveConstant("Surviving Leaves"));

        // query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001));

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void JoinEqualsBooksQuery()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            // ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, 
            [
                // new JoinDeclaration("a", "b", "id", "author", JoinType.InnerJoin)
            ]
        );
        query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.title"),
            new SelectionLeaveConstant("Surviving Leaves"));

        // query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001));

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
    void Join()
    {
        var query = new Query.Query();
        var tmp = new Dictionary<string, OneOf<Table, Query.Query>>
        {
            ["b"] = systemCatalog.Tables["books"],
            // ["a"] = systemCatalog.Tables["authors"]
        };
        query.From = new FromClause( tmp, 
            [
                // new JoinDeclaration("a", "b", "id", "author", JoinType.InnerJoin)
            ]
        );
        query.Selection = new EqualitySelectionOperator(new SelectionLeaveColumn("b.year"), new SelectionLeaveConstant(2001));

        // query.Selection = new IsNullSelectionOperator(new SelectionLeaveColumn("b.category"));
        var result = query.Execute();
        Console.WriteLine($"Result: {result.Count()} rows");
    }
    
}