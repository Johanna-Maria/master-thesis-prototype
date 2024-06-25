using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection.Leaves;

public class SelectionLeaveColumn(string name) :ISelectionLeave
{
    public string Name { get; set; } = name;
    
    public object? Calculate(Row row)
    {
        return row[Name];
    }

    public override string ToString()
    {
        return Name;
    }
}