using System.Collections;
using System.Diagnostics;

namespace SchmidiDB.Storage.Index;

public class HashTableIndex(string column, Table table)
{
    public string Column { get; } = column;

    public Table Table { get; } = table;

    public Dictionary<object, List<int>> _hashTable;

    private bool _isBuilt = false;

    public void Build()
    {
        if (_isBuilt) return;
        int idx = 0;
        var withIndex = Table.Select(row => new KeyValuePair<int, object?>(idx++, row[column])).ToList();
        _hashTable = 
            withIndex.GroupBy(pair => pair.Value)
                .Select(group => new KeyValuePair<object,List<int>>(group.Key ?? "null", group.Select(value => value.Key).ToList()))
                 .ToDictionary();    
        _isBuilt = true;
    }

    public IEnumerable<Row> Scan(object? key, Func<Row, bool> predicate)
    {
        if (!_isBuilt || !_hashTable.TryGetValue(key ?? "null", out var result)) return new List<Row>();
        return result.Select(id => Table.Data[id]).Where(predicate);
    }

    public IEnumerable<Row> Scan(object? key)
    {
        return this.Scan(key, (Row row) => true);
    }

    public override string ToString()
    {
        return string.Join("\n", _hashTable.Select(pair => pair.Key +" {"+ string.Join(" ", pair.Value)  +"}"));
    }
}