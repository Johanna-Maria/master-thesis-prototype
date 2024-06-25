using SchmidiDB.Query.Selection.Leaves;
using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection;

public class IsNotNullSelectionOperator(SelectionLeaveColumn column) : ISelectionOperator
{
    public SelectionLeaveColumn Column { get; set; } = column;

    public bool Calculate(Row row)
    {
        var val = Column.Calculate(row);
        return val is not null;
    }
    
    public double CalculateSelectivity(Dictionary<string, string> tableNames)
    {
        var systemCatalog = SystemCatalog.Instance;
        var columnName = Column.Name.Split(".");
        var tableName = tableNames[columnName[0]];
        return 1.0 - systemCatalog.GetSelectivity(tableName, columnName[^1], null);
    }
    
    public IEnumerable<KeyValuePair<string, object?>> GetPotentialIndexes()
    {
        return new List<KeyValuePair<string, object?>>();
    }

    public Dictionary<string, KeyValuePair<ISelectionOperator, double>> CanGetPreSelected(Dictionary<string, string> tableNames)
    {
        var systemCatalog = SystemCatalog.Instance;
        var columnName = Column.Name.Split(".");
        var tableName = tableNames[columnName[0]];
        return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>
        {
            {
                tableName,
                new (new IsNotNullSelectionOperator(new SelectionLeaveColumn(columnName[^1])),
                    1.0 - systemCatalog.GetSelectivity(tableName, columnName[^1], null)
                )
            }
        };
    }

    public override string ToString()
    {
        return Column + " IS NOT NULL ";
    }
}