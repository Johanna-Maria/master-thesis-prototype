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

public class JoinTestsEstimations
{

    public void EstimateShort()
    {
        TestDatabase.UseTestDatabase();
        SystemCatalog systemCatalog = SystemCatalog.Instance;
        systemCatalog.Analyze();
        systemCatalog.CreateIndex("nationality", "authors");
        systemCatalog.CreateIndex("category", "books");
        systemCatalog.CreateIndex("continent", "nationalities");
        var authors = systemCatalog.Tables["authors"];
        // books = systemCatalog.Tables["books"];
        var nationalities = systemCatalog.Tables["nationalities"];
        var continents = systemCatalog.Tables["continents"];
        List<IJoinOperator> joinOperators =
        [
            new NestedLoopJoinOperator(), new NestedLoopJoinWithIndexOperator(), new SortMergeJoinOperator(),
            new HashJoinOperator()
        ];
        foreach (var joinOperator in joinOperators)
        {
            var estimated = Convert.ToInt64(Math.Ceiling(authors.Count() * nationalities.Count() * 1.0 /
                                                         long.Max(systemCatalog.GetNumOfDistinctValuesFor("authors", "nationality"),
                                                             systemCatalog.GetNumOfDistinctValuesFor("nationalities", "id"))));
            var from = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
            {
                ["nationalities"] = systemCatalog.Tables["nationalities"],
                ["authors"] = systemCatalog.Tables["authors"]
            }, []);
            var joinDeclaration = new JoinDeclaration("authors", "nationalities", "nationality", "id", JoinType.InnerJoin);
            Console.WriteLine($"AuthorsJoinNationalities: {joinOperator} Estimated: {
                joinOperator.EstimateCosts(authors.Count(), nationalities.Count(), estimated, from, joinDeclaration, ["authors.id"], ["nationalities.id"])}");
            joinDeclaration = new JoinDeclaration("nationalities", "authors", "id", "nationality", JoinType.InnerJoin);
            Console.WriteLine($"NationalitiesJoinAuthors: {joinOperator} Estimated: {
                joinOperator.EstimateCosts(nationalities.Count(), authors.Count(), estimated, from, joinDeclaration, ["nationalities.id"], ["authors.id"])}");

            estimated = Convert.ToInt64(Math.Ceiling(nationalities.Count() * continents.Count() * 1.0 /
                                                         long.Max(systemCatalog.GetNumOfDistinctValuesFor("nationalities", "continent"),
                                                             systemCatalog.GetNumOfDistinctValuesFor("continents", "id"))));
            from = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
            {
                ["nationalities"] = systemCatalog.Tables["nationalities"],
                ["continents"] = systemCatalog.Tables["continents"]
            }, []);
            joinDeclaration = new JoinDeclaration("continents", "nationalities", "id", "continent", JoinType.InnerJoin);
            Console.WriteLine($"ContinentsJoinNationalities: {joinOperator} Estimated: {
                joinOperator.EstimateCosts(continents.Count(), nationalities.Count(), estimated, from, joinDeclaration, ["continents.id"], ["nationalities.id"])}");
            joinDeclaration = new JoinDeclaration("nationalities", "continents", "continent", "id", JoinType.InnerJoin);
            Console.WriteLine($"NationalitiesJoinContinents: {joinOperator} Estimated: {
                joinOperator.EstimateCosts(nationalities.Count(), continents.Count(), estimated, from, joinDeclaration, ["nationalities.id"], ["continents.id"])}");
        }
    }
    public void EstimateAll()
    {
        TestDatabase.UseTestDatabase();
        SystemCatalog systemCatalog = SystemCatalog.Instance;
        systemCatalog.Analyze();
        systemCatalog.CreateIndex("author", "books");
        var authors = systemCatalog.Tables["authors"];
        var books = systemCatalog.Tables["books"];
        var authorsUnordered = authors.OrderBy(row => row["firstName"]).ToList();
        var booksOrdered = books.OrderBy(row => row["author"]).ToList();
        List<IJoinOperator> joinOperators =
        [
            new NestedLoopJoinOperator(), new NestedLoopJoinWithIndexOperator(), new SortMergeJoinOperator(),
            new HashJoinOperator()
        ];
        foreach (var joinOperator in joinOperators)
        {
            var from = new FromClause(new Dictionary<string, OneOf<Table, Query.Query>>
            {
                ["books"] = systemCatalog.Tables["books"],
                ["authors"] = systemCatalog.Tables["authors"]
            }, []);
            var aCount = Convert.ToInt64(authors.Count());
            var bCount = Convert.ToInt64(books.Count());
            var estimated = Convert.ToInt64(Math.Ceiling(bCount * aCount * 1.0 /
                                                     long.Max(systemCatalog.GetNumOfDistinctValuesFor("books", "author"),
                                                         systemCatalog.GetNumOfDistinctValuesFor("authors", "id"))));
            List<string> orderedBooks = ["books.id"];
            List<string> orderedAuthors = ["authors.id"];
            var joinDeclaration = new JoinDeclaration("authors", "books", "id", "author", JoinType.InnerJoin);
            Console.WriteLine($"AuthorsJoinBooks: {joinOperator} Estimated: {joinOperator.EstimateCosts(authors.Count(), books.Count(), estimated, from, joinDeclaration, orderedAuthors, orderedBooks)}");
            joinDeclaration = new JoinDeclaration("books", "authors", "author", "id", JoinType.InnerJoin);
            Console.WriteLine($"BooksJoinAuthors: {joinOperator} Estimated: {joinOperator.EstimateCosts(books.Count(), authors.Count(), estimated,
                from, joinDeclaration, orderedBooks, orderedAuthors)}");
            aCount = Convert.ToInt64(authors.Count());
            bCount = Convert.ToInt64(books.Count());
            estimated = Convert.ToInt64(Math.Ceiling(bCount * aCount * 1.0 /
                                                     long.Max(systemCatalog.GetNumOfDistinctValuesFor("books", "author"),
                                                         systemCatalog.GetNumOfDistinctValuesFor("authors", "id"))));
            orderedBooks = ["books.author"];
            joinDeclaration = new JoinDeclaration("authors", "books", "id", "author", JoinType.InnerJoin);
            Console.WriteLine($"AuthorsJoinBooksOrdered: {joinOperator} Estimated: {joinOperator.EstimateCosts(authors.Count(), books.Count(), estimated, from, joinDeclaration, orderedAuthors, orderedBooks)}");
            joinDeclaration = new JoinDeclaration("books", "authors", "author", "id", JoinType.InnerJoin);
            Console.WriteLine($"BooksJoinAuthorsOrdered: {joinOperator} Estimated: {joinOperator.EstimateCosts(books.Count(), authors.Count(), estimated, from, joinDeclaration, orderedBooks, orderedAuthors)}");
            aCount = Convert.ToInt64(authors.Count());
            bCount = Convert.ToInt64(books.Count());
            estimated = Convert.ToInt64(Math.Ceiling(bCount * aCount * 1.0 /
                                                     long.Max(systemCatalog.GetNumOfDistinctValuesFor("books", "author"),
                                                         systemCatalog.GetNumOfDistinctValuesFor("authors", "id"))));
            orderedBooks = ["books.id"];
            orderedAuthors = ["authors.firstName"];
            joinDeclaration = new JoinDeclaration("authors", "books", "id", "author", JoinType.InnerJoin);
            Console.WriteLine($"AuthorsJoinBooksUnordered: {joinOperator} Estimated: {joinOperator.EstimateCosts(authors.Count(), books.Count(), estimated, from, joinDeclaration, orderedAuthors, orderedBooks)}");
            joinDeclaration = new JoinDeclaration("books", "authors", "author", "id", JoinType.InnerJoin);
            Console.WriteLine($"BooksJoinAuthorsUnordered: {joinOperator} Estimated: {joinOperator.EstimateCosts(books.Count(), authors.Count(), estimated, from, joinDeclaration, orderedBooks, orderedAuthors)}");
        }
    }
}