using System.Diagnostics;
using SchmidiDB.Query.Join.Tree;
using SchmidiDB.Query.Selection;
using SchmidiDB.Storage;
using OneOf;
using SchmidiDB.Query.Access;
using SchmidiDB.Query.Join;
using SchmidiDB.Query.Join.FromClause;
using SchmidiDB.Storage.System;
using SchmidiDB.Tests.Util;

namespace SchmidiDB.Query;

public class Query
{
    public List<string> Projection
    {
        get
        {
            if (_projection.Count == 0)
            {
                _projection = From.Data.Select(element => element.Value
                    .MapT0(table =>
                        SystemCatalog.Instance.TableInfos[table.Name].Columns.Select(col => element.Key + "." + col))
                    .MapT1(query => query.Projection.Select(col => element.Key + "." + col)))
                    .SelectMany(list => (IEnumerable<string>) list.Value)
                    .ToList();
            }

            return _projection;
        }
        set => _projection = value;
    }

    private List<string> _projection = [];

    public ISelectionOperator? Selection { get; set; }

    public FromClause From;

    private JoinTree? BestJoinTree { get; set; } = null;

    private long EstimatedNumberOfRows { get; set; } = long.MaxValue;
    
    public List<KeyValuePair<JoinTree, KeyValuePair<long,double>>> AllJoinTreeResults { get; set; }

    
    public void PickQueryPlan(bool output = true, bool withPreselection = true)
    {
        if (output)
        {
            Console.WriteLine("----------------------------Query Plan----------------------------");
            Console.WriteLine("Preselection:");
        }
        var tableNames = From.GetNewNamesOfTables();
        if (Selection is not null && withPreselection)
        {
            var preSelections = Selection.CanGetPreSelected(tableNames)
                .Select(pair => new KeyValuePair<string,Func<Row, bool>>(pair.Key, (Row row) => pair.Value.Key.Calculate(row)))
                .ToDictionary();
            var potentialIndexes = Selection.GetPotentialIndexes();
            var selectivities = Selection.CanGetPreSelected(tableNames)
                .Select(pair => new KeyValuePair<string,double>(pair.Key, pair.Value.Value))
                .ToDictionary();
            From.PickAccessOperators(preSelections, selectivities, potentialIndexes);
        }
        else
        {
            From.PreSelections = new Dictionary<string, KeyValuePair<Func<Table>, double>>();
        }
        if (output)
        {
            From.PrintPreSelection();
        }

        EstimatedNumberOfRows = EstimateNumberOfRows();
        var bestJoinTree = GetBestJoinTree();
        if (output)
        {
            Console.WriteLine(bestJoinTree?.PrintJoinTree());
            Console.WriteLine("Estimated Number of Rows: " + EstimatedNumberOfRows);
            Console.WriteLine("------------------------------------------------------------------"); 
        }
    }

    public IEnumerable<Row> Execute(bool printQueryPlan = true)
    {
        PickQueryPlan(printQueryPlan);
        From.ExecutePreSelections();
        var bestJoinTree = GetBestJoinTree();
        var prefixRow = (Row row, string? prefix) =>
        {
            if (prefix is null) return row;
            return new Row(row.Select(pair => new KeyValuePair<string, object>($"{prefix}.{pair.Key}", pair.Value)));
        };
        var from = bestJoinTree?.Execute() ?? ((IEnumerable<Row>) From.Data.First().Value.MapT1(qu => qu.Execute()).Value).Select(row => prefixRow(row, From.Data.First().Key)).ToList();
        var seqScan = new SequentialScanAccessOperator();
        var selectionCall = (Row row) => true;
        if (Selection != null)
        {
            selectionCall = (Row row) => Selection.Calculate(row);
        }
        var selection = seqScan.Scan(from, selectionCall);
        if (!Projection.Any())
        {
            return selection;
        }

        return selection.Select(row =>
        {
            var newRow = new Row();
            foreach (var pair in row)
            {
                if (Projection.Contains(pair.Key))
                {
                    newRow.Add(pair.Key, pair.Value);
                }
            }
            return newRow;
        });
    }

    public long EstimateNumberOfRows()
    {
        if (EstimatedNumberOfRows == long.MaxValue)
        {
            if (From.Data.Keys.Count < 2)
            {
                long numOfRows = Convert.ToInt64(From.Data.First().Value.MapT0(table => table.Data.Count)
                    .MapT1(query => query.EstimateNumberOfRows()).Value);
                EstimatedNumberOfRows = Selection is null ? numOfRows : (long) Math.Ceiling(numOfRows * Selection.CalculateSelectivity(From.GetNewNamesOfTables()));
            }
            else
            {
                var allJoinOrderings = GetAllJoinOrderings();
                bool isFirst = true;
                long leftNumOfRows = 0;
                long estimatedNumOfRows = 0;
                foreach (var joinDeclaration in allJoinOrderings.First())
                {
                    var leftRelation = From.Data[joinDeclaration.LeftRelation];
                    var rightRelation = From.Data[joinDeclaration.RightRelation];
                    if (isFirst)
                    {
                        leftNumOfRows = (long) leftRelation.MapT0(table => Convert.ToInt64(table.Data.Count))
                            .MapT1(query => query.EstimateNumberOfRows()).Value;
                    }

                    estimatedNumOfRows = EstimateJoinSelectivity(isFirst, leftRelation, rightRelation,
                        leftNumOfRows, joinDeclaration, false);
                    leftNumOfRows = estimatedNumOfRows;
                    isFirst = false;
                }

                EstimatedNumberOfRows = Selection is null ? estimatedNumOfRows : (long) Math.Ceiling(estimatedNumOfRows *
                                                                                                        Selection.CalculateSelectivity(
                                                                                                            From.GetNewNamesOfTables()));
            }
        }
        return EstimatedNumberOfRows;
    }

    private List<string> _orderedBy = new List<string>();
    public IEnumerable<string> GetOrderedBy()
    {
        if (_orderedBy.Count == 0)
        {
            GetBestJoinTree();
        }
        return _orderedBy;
    }

    public void PrintJoinTree()
    {
        PickQueryPlan(false);
        var joinTree = GetBestJoinTree();
        if (joinTree == null)
        {
            Console.WriteLine("Query has no join tree");
            return;
        }
        Console.WriteLine(joinTree.PrintJoinTree());
    }
    

    private JoinTree? GetBestJoinTree()
    {
        // PreSelectTables();
        if (BestJoinTree is not null) { return BestJoinTree;}
        //if there is no join
        if (From.Data.Count < 2)
        {
            return null;
        }
        var systemCatalog = SystemCatalog.Instance;
        var joinOperators = new List<IJoinOperator> {new NestedLoopJoinOperator(), new NestedLoopJoinWithIndexOperator(),new SortMergeJoinOperator(), new HashJoinOperator()};
        // var joinOperators = new List<IJoinOperator> {new NestedLoopJoinWithIndexOperator(),  new SortMergeJoinOperator(), new HashJoinOperator()};
        List<KeyValuePair<JoinTree, KeyValuePair<long,double>>> costsPerTree = new List<KeyValuePair<JoinTree, KeyValuePair<long,double>>>();
        foreach (var permutation in GetAllJoinOrderings())
        {
            var leftOrderedBy = new List<string>();
            var rightOrderedBy = new List<string>();
            JoinTree? joinTree = null;
            long leftNumOfRows = long.MinValue;
            bool isFirst = true;
            double sum = 0.0;
            foreach (var joinDeclaration in permutation)
            {
                var leftRelation = From.Data[joinDeclaration.LeftRelation];
                var rightRelation = From.Data[joinDeclaration.RightRelation];
                if (isFirst)
                {
                    leftOrderedBy.AddRange(leftRelation.IsT0 ? new List<string>{systemCatalog.TableInfos[leftRelation.AsT0.Name].PrimaryKey} : leftRelation.AsT1.GetOrderedBy());
                }
                rightOrderedBy = (rightRelation.IsT0 ? new List<string>{systemCatalog.TableInfos[rightRelation.AsT0.Name].PrimaryKey} : rightRelation.AsT1.GetOrderedBy()).ToList();
                if (isFirst)
                {
                    leftOrderedBy = leftOrderedBy.Select(column => joinDeclaration.LeftRelation+"."+column).ToList();
                }
                rightOrderedBy = rightOrderedBy.Select(column => joinDeclaration.RightRelation+"."+column).ToList();
                var leftNum = EstimateLeftNumOfRows(isFirst, leftRelation, leftNumOfRows, joinDeclaration);
                var rightNum = EstimateRightNumOfRows(rightRelation, joinDeclaration);
                var estimatedNumOfRows = EstimateJoinSelectivity(isFirst, leftRelation, rightRelation, leftNumOfRows, joinDeclaration);
                var bestCosts = Double.MaxValue;
                IJoinOperator bestOperator = new NestedLoopJoinOperator();
                foreach (var joinOperator in joinOperators)
                {
                    var costs = joinOperator.EstimateCosts(leftNum,
                        rightNum, estimatedNumOfRows, From, joinDeclaration, leftOrderedBy, rightOrderedBy);
                        if (costs < bestCosts)
                        {
                            bestCosts = costs;
                            bestOperator = joinOperator;
                        }
                }
                OneOf<JoinTree, Table, Query> leftSide = isFirst ? leftRelation.IsT0 ? leftRelation.AsT0 : From.Data[joinDeclaration.LeftRelation].AsT1 : joinTree;
                joinTree = new JoinTree(
                    From,
                    leftSide,
                    rightRelation.IsT0 ? rightRelation.AsT0 : rightRelation.AsT1,
                    joinDeclaration.JoinType, 
                    isFirst ? joinDeclaration.LeftJoinKey : joinDeclaration.LeftRelation+"."+joinDeclaration.LeftJoinKey, 
                    joinDeclaration.RightJoinKey,
                    bestOperator,
                    isFirst ? joinDeclaration.LeftRelation : null,
                    joinDeclaration.RightRelation,
                    leftOrderedBy,
                    rightOrderedBy);
                leftOrderedBy = bestOperator
                    .GetOrderedByAfterJoin(leftOrderedBy, rightOrderedBy, joinDeclaration);
                leftNumOfRows = estimatedNumOfRows;
                isFirst = false;
                sum += bestCosts;
            }
            costsPerTree.Add(new KeyValuePair<JoinTree, KeyValuePair<long,double>>(joinTree, new KeyValuePair<long, double>(leftNumOfRows, sum)));
        }
        var bestResult = costsPerTree.MinBy(pair => pair.Value.Value);
        //Kept for evaluation
        AllJoinTreeResults = costsPerTree;
        BestJoinTree = bestResult.Key;
        _orderedBy = BestJoinTree.JoinOperator.GetOrderedByAfterJoin(BestJoinTree.LeftOrderedBy, BestJoinTree.RightOrderedBy, BestJoinTree.LeftRelationName, BestJoinTree.RightRelationName, BestJoinTree.LeftJoinColumn, BestJoinTree.RightJoinColumn, BestJoinTree.JoinType);
        return BestJoinTree;
    }

    private long EstimateLeftNumOfRows(bool isFirst, OneOf<Table, Query> leftRelation, long leftNumOfRows,
        JoinDeclaration joinDeclaration, bool withPreSelections = true)
    {
        return  isFirst ? leftRelation.IsT0
            ? (long) Math.Ceiling(leftRelation.AsT0.Count() * (withPreSelections && From.PreSelections is not null && From.PreSelections.ContainsKey(joinDeclaration.LeftRelation) ? From.PreSelections[joinDeclaration.LeftRelation].Value : 1.0))
            : leftRelation.AsT1.EstimateNumberOfRows() : leftNumOfRows;
    }

    private long EstimateRightNumOfRows(OneOf<Table, Query> rightRelation, JoinDeclaration joinDeclaration, bool withPreSelections = true)
    {
        return rightRelation.IsT0
            ? (long) Math.Ceiling(rightRelation.AsT0.Count() * (withPreSelections && From.PreSelections is not null && From.PreSelections.ContainsKey(joinDeclaration.RightRelation) ? From.PreSelections[joinDeclaration.RightRelation].Value : 1.0))
            : rightRelation.AsT1.EstimateNumberOfRows();
    }

    public long EstimateJoinSelectivity(bool isFirst, OneOf<Table, Query> leftRelation, OneOf<Table, Query> rightRelation, long leftNumOfRows, JoinDeclaration joinDeclaration, bool withPreSelection = true)
    {
        var systemCatalog = SystemCatalog.Instance;
        long leftNum = EstimateLeftNumOfRows(isFirst, leftRelation, leftNumOfRows, joinDeclaration, withPreSelection);
        long rightNum = EstimateRightNumOfRows(rightRelation, joinDeclaration, withPreSelection);
        // string leftTableName = From.Data[joinDeclaration.LeftRelation.Split(".")[0]]From.Data[joinDeclaration.LeftRelation.Split(".")[0]].AsT0.Name;
        double leftSelectivity = isFirst
            ? (double) leftRelation
                .MapT0(table => 1.0 / systemCatalog.GetNumOfDistinctValuesFor(table.Name, joinDeclaration.LeftJoinKey))
                .MapT1(query => SystemParameters.DefaultJoinSelectivity).Value 
            : (double) From.Data[joinDeclaration.LeftRelation.Split(".")[0]]
                .MapT0(table => 1.0 / systemCatalog.GetNumOfDistinctValuesFor(table.Name, joinDeclaration.LeftJoinKey))
                .MapT1(query => SystemParameters.DefaultJoinSelectivity).Value;
        double rightSelectivity = (double) rightRelation
            .MapT0(table =>
                1.0 / systemCatalog.GetNumOfDistinctValuesFor(table.Name, joinDeclaration.RightJoinKey))
            .MapT1(query => SystemParameters.DefaultJoinSelectivity).Value;
        long leftNullRows = 0;
        double leftNullFraction = isFirst
            ? (double) leftRelation
                .MapT0(table => systemCatalog.Statistics[table.Name].NullFraction[joinDeclaration.LeftJoinKey])
                .MapT1(query => 0.0).Value
            : (From.Data[joinDeclaration.LeftRelation.Split(".")[0]].IsT0
                ? systemCatalog.Statistics[From.Data[joinDeclaration.LeftRelation.Split(".")[0]].AsT0.Name]
                    .NullFraction[joinDeclaration.LeftJoinKey]
                : 0.0);
        double rightNullFraction = (double) rightRelation.MapT0(table =>
            systemCatalog.Statistics[table.Name].NullFraction[joinDeclaration.RightJoinKey]).MapT1(query => 0.0).Value;
        if (leftNullFraction > 0.0)
        {
            var nullRows = (long) Math.Floor(leftNum * leftNullFraction);
            // leftNum -= nullRows;
            leftNullRows = joinDeclaration.JoinType is JoinType.OuterJoin or JoinType.LeftOuterJoin
                ? nullRows
                : 0;
        }
        
        long rightNullRows = 0;
        if (rightNullFraction > 0.0)
        {
            var nullRows = (long) Math.Floor(rightNum * rightNullFraction);
            // rightNum -= nullRows;
            rightNullRows = joinDeclaration.JoinType is JoinType.OuterJoin or JoinType.RightOuterJoin
                ? nullRows
                : 0;
        }

        return Convert.ToInt64(
            Math.Ceiling(
                leftNum * rightNum * Double.Min(leftSelectivity, rightSelectivity) * (1 - leftNullFraction) * (1 - rightNullFraction)))
               + leftNullRows + rightNullRows;
    }
    
    private List<List<JoinDeclaration>> GetPermutations(List<JoinDeclaration> list)
    {
        if (list.Count() == 1)
            return new List<List<JoinDeclaration>> { list };

        return list.SelectMany(e =>
                GetPermutations(list.Where(x => !x.Equals(e)).ToList()),
            (e, c) => new[] { e }.Concat(c).ToList()).ToList();
    }

    private List<List<JoinDeclaration>> WrongOuterJoinOrders(List<List<JoinDeclaration>> allJoinOrderings)
    {
        //outer joins need to stay at their initial positions
        var outerJoins = From.JoinDeclarations
            .Select((declaration, index) => new { Declaration = declaration, Index = index })
            .Where(pair => pair.Declaration.JoinType != JoinType.InnerJoin)
            // .Select(pair => pair.Index)
            .ToList();
        var falseOrderings = new List<List<JoinDeclaration>>();
        if (outerJoins.Any())
        {
            var originalOrdering = From.JoinDeclarations
                .Select((declaration, index) => new {Declaration = declaration, Index = index})
                .Where(pair => pair.Declaration.JoinType == JoinType.InnerJoin)
                .Select((declaration, index) => new
                    {Declaration = declaration, NextOuterJoin = (outerJoins.FirstOrDefault(pair => pair.Index > index, null))?.Index ?? int.MaxValue})
                .ToList();
            for (int variantNr = 0; variantNr < allJoinOrderings.Count; variantNr++)
            {
                var variantOuterJoins = allJoinOrderings[variantNr]
                    .Select((declaration, index) => new { Declaration = declaration, Index = index })
                    .Where(pair => pair.Declaration.JoinType != JoinType.InnerJoin)
                    .ToList();
                if (!variantOuterJoins.SequenceEqual(outerJoins)) falseOrderings.Add(allJoinOrderings[variantNr]);
                var variantOrdering = From.JoinDeclarations
                    .Select((declaration, index) => new {Declaration = declaration, Index = index})
                    .Where(pair => pair.Declaration.JoinType == JoinType.InnerJoin)
                    .Select((declaration, index) => new
                        {Declaration = declaration, NextOuterJoin = (outerJoins.FirstOrDefault(pair => pair.Index > index, null))?.Index ?? int.MaxValue})
                    .ToList();
                for (int i = 0; i < variantOrdering.Count; i++)
                {
                    var originalPair = originalOrdering.First(pair => pair.Declaration.Equals(variantOrdering[i].Declaration));
                    if (originalPair.NextOuterJoin != variantOrdering[i].NextOuterJoin) falseOrderings.Add(allJoinOrderings[variantNr]);
                }
            }
        }
        return falseOrderings;
    }

    private List<IEnumerable<JoinDeclaration>>? _allJoinOrderings = null;
    private List<IEnumerable<JoinDeclaration>> GetAllJoinOrderings()
    {
        if (_allJoinOrderings is not null)
        {
            return _allJoinOrderings;
        }
        List<List<JoinDeclaration>> allJoinOrderings = GetPermutations(From.JoinDeclarations).ToList();
        //remove permutations where the join order is invalid due to outer joins
        allJoinOrderings = allJoinOrderings.Except(WrongOuterJoinOrders(allJoinOrderings)).ToList();
        //remove join orders with cartesian product and swap tables if necessary
        var toBeRemoved = new List<IEnumerable<JoinDeclaration>>();
        for (int variantNr = 0; variantNr < allJoinOrderings.Count; variantNr++)
        {
            var variant = allJoinOrderings[variantNr];
            var usedTables = new List<string>();
            for (int joinDecNum = 0; joinDecNum < variant.Count; joinDecNum++)
            {
                var joinDeclaration = variant[joinDecNum];
                if (! usedTables.Any()) usedTables.AddRange(new List<string>(){joinDeclaration.LeftRelation, joinDeclaration.RightRelation});
                else if (!usedTables.Contains(joinDeclaration.LeftRelation) && !usedTables.Contains(joinDeclaration.RightRelation))
                {
                    toBeRemoved.Add(variant);
                    break;
                }
                else if (!usedTables.Contains(joinDeclaration.LeftRelation))
                {
                    usedTables.Add(joinDeclaration.LeftRelation);
                    var switchesJoinType = joinDeclaration.JoinType is JoinType.LeftOuterJoin
                        ?
                        JoinType.RightOuterJoin
                        : joinDeclaration.JoinType is JoinType.RightOuterJoin
                            ? JoinType.LeftOuterJoin
                            : joinDeclaration.JoinType;
                    allJoinOrderings[variantNr][joinDecNum] = new JoinDeclaration(
                        joinDeclaration.RightRelation, joinDeclaration.LeftRelation, joinDeclaration.RightJoinKey,
                        joinDeclaration.LeftJoinKey, switchesJoinType);
                }
                else if (!usedTables.Contains(joinDeclaration.RightRelation)) usedTables.Add(joinDeclaration.RightRelation);
            }
        }

        _allJoinOrderings = allJoinOrderings.Except(toBeRemoved).ToList();
        return _allJoinOrderings;
    }
    public List<JoinTreeResult> PrintAndExecuteAllJoinTrees(bool execute = true)
    {
        List<JoinTreeResult> estimationResults = [];
        foreach (var pair in AllJoinTreeResults.OrderBy(entry => entry.Value.Value))
        {
            Console.WriteLine(pair.Key.ShortDescription());
            Console.WriteLine(pair.Key.ShortDescriptionJoinTypes());
            Console.WriteLine(pair.Key.PrintJoinTree());
            Console.WriteLine(pair.Value.Key+" Rows, Estimated Costs: "+pair.Value.Value);
            estimationResults.Add(new JoinTreeResult
            {
                Costs = pair.Value.Value, Rows = pair.Value.Key, JoinShort = pair.Key.ShortDescription(),
                JoinLong = pair.Key.ToString(), JoinOperators = pair.Key.ShortDescriptionJoinTypes()
            });
            if (execute)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                pair.Key.Execute().ToList();
                stopwatch.Stop();
                TimeSpan duration = stopwatch.Elapsed;
                Console.WriteLine("Duration: " + duration.TotalMilliseconds + " milliseconds");
            }
        }

        return estimationResults;
    }
    
    public override string ToString()
    {
        string select = "SELECT " + (!Projection.Any() ? "*" : String.Join(", ", Projection));
        string from = "FROM" + (GetBestJoinTree() is null ? (" " + From.Data.First().Value.MapT1(q => "( "+q+")").MapT0(t => t.Name).Value + " " + From.Data.First().Key) : BestJoinTree.ToString()); //+ (string) From.MapT0(table => ((Table) table).ToString()).MapT1(tree => tree.ToString())
            //.MapT2(query => query.ToString()).Value;
        string where = Selection is not null ?  "WHERE " + Selection : "";
        return select + "\n" + from + "\n" + where;
    }
    
}