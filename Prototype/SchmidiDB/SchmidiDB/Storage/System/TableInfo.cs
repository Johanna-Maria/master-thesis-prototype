namespace SchmidiDB.Storage.System;

public class TableInfo(string primaryKey, IEnumerable<string> columns)
{
    public string PrimaryKey { get; set; } = primaryKey;

    public IEnumerable<string> Columns { get; set; } = columns;
}