using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection.Leaves;

public interface ISelectionLeave
{
    // public object? Value { get; set; }

    public object? Calculate(Row row);

}