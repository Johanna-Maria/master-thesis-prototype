namespace SchmidiDB.Storage;

public class Row : Dictionary<String, object?>
{
    
    
    public Row() : base() { }

    public Row(IEnumerable<KeyValuePair<string, object?>> pairs) : base()
    {
        foreach (var pair in pairs)
        {
            this.Add(pair.Key, pair.Value);
        }
    }

    public Row(Row leftRow, Row rightRow, string? leftPrefix = null, string? rightPrefix = null) : base()
    {
        if (leftPrefix is not null && rightPrefix is not null && leftPrefix.Equals(rightPrefix))
        {
            throw new Exception("Relations need different names");
        }
        foreach (var keyValuePair in leftRow.ToDictionary(pair => leftPrefix is null ? pair.Key : leftPrefix + "." + pair.Key, pair => pair.Value).Union(rightRow.ToDictionary(pair => rightPrefix is null ? pair.Key : rightPrefix + "." + pair.Key, pair => pair.Value)))
        {
            this.Add(keyValuePair.Key, keyValuePair.Value);
        }
    }

    public Row GenerateNullRow()
    {
        var nullRow = new Row();
        foreach (var pair in this)
        {
            nullRow.Add(pair.Key, null);
        }

        return nullRow;
    }

    public override string ToString()
    {
        return string.Join(", ", this.Select(pair => $"[{pair.Key}]: {pair.Value ?? "null"}").ToArray());
    }
}