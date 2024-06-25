using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection.Leaves;

public class SelectionLeaveConstant(object value) : ISelectionLeave
{
    public object Value { get; set; } = value;
    
    public object Calculate(Row row)
    {
        return Value;
    }

    public override string ToString()
    {
        return Value is string ? "'"+Value+"'" : Value.ToString();
    }
}