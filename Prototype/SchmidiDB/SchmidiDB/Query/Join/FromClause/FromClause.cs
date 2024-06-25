using OneOf;
using SchmidiDB.Query.Access;
using SchmidiDB.Storage;
using SchmidiDB.Storage.System;

namespace SchmidiDB.Query.Join.FromClause;

public class FromClause(Dictionary<string, OneOf<Table, Query>> data, List<JoinDeclaration> joinDeclarations)
{
    public Dictionary<string, OneOf<Table, Query>> Data { get; set; } = data;

    public List<JoinDeclaration> JoinDeclarations { get; set; } = joinDeclarations;

    //Dictionary Key stors the table name
    //Function stores the Scan
    //double is the estimated selectivity
    public Dictionary<string, KeyValuePair<Func<Table>, double>>? PreSelections { get; set; } = null;
    
    private Dictionary<string, string> PreSelectionsOutput { get; set; } = new Dictionary<string, string>();

    public Dictionary<string, string> GetNewNamesOfTables()
    {
        return Data.Where(pair => pair.Value.IsT0).Select(pair => new KeyValuePair<string,string>(pair.Key, pair.Value.AsT0.Name)).ToDictionary();
    }

    public void PickAccessOperators(Dictionary<string, Func<Row, bool>> predicates, Dictionary<string, double> selectivities,
        IEnumerable<KeyValuePair<string, object?>> indexes)
    {
        if (PreSelections is not null && PreSelections.Count != 0)
        {
            return;
        }

        PreSelections = new Dictionary<string, KeyValuePair<Func<Table>, double>>();
        foreach (var pair in Data.Where(p => p.Value.IsT0))
        {
            var systemCatalog = SystemCatalog.Instance;
            var originalTableName = pair.Value.AsT0.Name;
            var tableName = pair.Key;
            var table = pair.Value.AsT0;
            if (table != null && predicates.ContainsKey(originalTableName))
            {
                var indexesForTable = indexes.Where(indexPair => indexPair.Key.Split(".")[0] == tableName).ToArray();
                var indexesAndSelectivitiesSorted = indexesForTable
                    .Where(p => systemCatalog.GetIndex(originalTableName, p.Key.Split(".")[^1]) != null)
                    .Select(indexPair => new KeyValuePair<KeyValuePair<string, object?>, double>(indexPair,
                        systemCatalog.GetSelectivity(originalTableName, indexPair.Key.Split(".")[^1], indexPair.Value)))
                    .OrderBy(pairPair => pairPair.Value)
                    .ToList();
                var indexesSorted = indexesAndSelectivitiesSorted.Select(pairPair => pairPair.Key).ToList();
                var selectivitiesSorted = indexesAndSelectivitiesSorted.Select(pairPair => pairPair.Value).ToList();
                if (indexesSorted.Any()
                    && SystemCatalog.Instance.GetIndex(originalTableName, indexesSorted[0].Key.Split(".")[^1]) != null
                    && selectivitiesSorted.First() < SystemParameters.IndexScanThreshold)
                {
                    var bestIndex = indexesSorted[0];
                    PreSelections[tableName] = new KeyValuePair<Func<Table>, double>(() => new Table(originalTableName,
                        SystemCatalog.Instance.GetIndex(originalTableName, bestIndex.Key.Split(".")[^1])
                            .Scan(bestIndex.Value, predicates[originalTableName]))
                        , selectivities[originalTableName]) ;
                    PreSelectionsOutput[tableName] =
                        $"Index Scan on ({bestIndex.Key}) = {bestIndex.Value}, estimated selectivity: {selectivities[originalTableName]}";
                }
                else
                {
                    PreSelections[tableName] = new KeyValuePair<Func<Table>, double>(() => new Table(originalTableName,
                        new SequentialScanAccessOperator().Scan(table, predicates[originalTableName])), selectivities[originalTableName]) ;
                    PreSelectionsOutput[tableName] = $"Sequential Scan, estimated selectivity: {selectivities[originalTableName]}";
                }
            }
        }
    }

    public void PrintPreSelection()
    {
        if (PreSelections is not null)
        {
            foreach (var preSelection in PreSelectionsOutput)
            {
                Console.WriteLine($"Table {preSelection.Key}: {preSelection.Value}");
            }
        }
    }

    public void ExecutePreSelections()
    {
        if (PreSelections is null)
        {
            throw new Exception("Preselection has not been evaluated!");
        }
        foreach (var preSelection in PreSelections)
        {
            Data[preSelection.Key] = preSelection.Value.Key();
        }
    }
}