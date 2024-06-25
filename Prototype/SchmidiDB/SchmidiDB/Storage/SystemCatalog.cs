using System.Management;
using SchmidiDB.Storage.Index;
using SchmidiDB.Storage.System;

namespace SchmidiDB.Storage;

public class SystemCatalog
{
    private SystemCatalog()
    {
        Tables = new Dictionary<string, Table>();
        Indexes = new Dictionary<string, List<HashTableIndex>>();
        TableInfos = new Dictionary<string, TableInfo>();
        Statistics = new Dictionary<string, Statistics>();
    }

    public static SystemCatalog Delete()
    {
        _systemCatalog = new SystemCatalog();
        return _systemCatalog;
    }

    private static SystemCatalog? _systemCatalog;

    public static SystemCatalog Instance => _systemCatalog ??= new SystemCatalog();

    public Dictionary<string, Table> Tables { get; }
    
    public Dictionary<string, TableInfo> TableInfos { get; }
    
    public Dictionary<String, List<HashTableIndex>> Indexes { get; set; }
    

    public HashTableIndex? GetIndex(string tableName, string indexColumn)
    {
        if (!Indexes.TryGetValue(tableName, out var indexList)) return null;
        return indexList.Find(index => index.Column == indexColumn);
    }

    public bool CreateTable(string tableName, IEnumerable<Row> data)
    {
        if (Tables.ContainsKey(tableName)) return false;
        var rows = data.ToList();
        Tables[tableName] = new Table(tableName, rows);
        var columns = rows.First().Select(pair => pair.Key).ToList();
        TableInfos[tableName] = new TableInfo(columns.First(), columns);
        Indexes[tableName] = new List<HashTableIndex>();
        CreateIndex(columns.First(), tableName);
        return true;
    }

    public bool CreateIndex(string column, string tableName)
    {
        if (!Tables.TryGetValue(tableName, out var table)) return false;
        var index = new HashTableIndex(column, table);
        index.Build();
        Indexes[tableName].Add(index);
        return true;
    }

    public Dictionary<string, Statistics> Statistics { get; }

    public void Analyze()
    {
        foreach (var tableInfo in TableInfos)
        {
            var statistic = new Statistics(tableInfo.Key, new Dictionary<string, int>(),
                new Dictionary<string, Dictionary<object, double>>(), new Dictionary<string, double>());
            foreach (var column in tableInfo.Value.Columns)
            {
                var numOfRows = Tables[tableInfo.Key].Count();
                var groupedValues = Tables[tableInfo.Key].Select(row => row[column]).GroupBy(value => value).ToList();
                statistic.NumOfDistinctValues[column] = groupedValues.Where(group => group.Key is not null).Count();
                statistic.NullFraction[column] = groupedValues.Where(group => group.Key is null).Count() > 0 ?
                    groupedValues.Where(group => group.Key is null).Select(group => 1.0 * group.Count() / numOfRows).First() : 0.0;
                if (tableInfo.Value.PrimaryKey == column) continue;
                statistic.MostCommonValues[column] = groupedValues
                    .Where(group => group.Count() > 1 && group.Key is not null)
                    .OrderByDescending(group => group.Count())
                    .Select(group => new KeyValuePair<object,double>(group.Key, 1.0 * group.Count() / numOfRows))
                    .Take(SystemParameters.MostCommonValuesThreshold)
                    .ToDictionary();
            }

            Statistics[tableInfo.Key] = statistic;
        }
    }

    public double GetSelectivity(string tableName, string columnName, object? value)
    {
        //https://www.postgresql.org/docs/current/row-estimation-examples.html
        if (TableInfos[tableName].PrimaryKey == columnName)
        {
            return 1.0 / Tables[tableName].Count();
        }
        if (value is null)
        {
            return Statistics[tableName].NullFraction[columnName];
        }
        bool containsValue = Statistics[tableName].MostCommonValues[columnName].TryGetValue(value, out double selectivity);
        if (containsValue)
        {
            return selectivity;
        }
        var selectivities = Statistics[tableName].MostCommonValues[columnName].Select(pair => pair.Value).ToList();
        var sum = selectivities.Aggregate(0.0, (l, r) => l + r) + Statistics[tableName].NullFraction[columnName];
        return (1.0 - sum) / (Statistics[tableName].NumOfDistinctValues[columnName] - selectivities.Count());
    }
    
    public int GetNumOfDistinctValuesFor(string tableName, string columnName)
    {
        return Statistics[tableName].NumOfDistinctValues[columnName];
    }
}