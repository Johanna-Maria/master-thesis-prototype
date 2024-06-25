using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection;

public interface ISelectionOperator
{
    public bool Calculate(Row row);

    public IEnumerable<KeyValuePair<string, object?>> GetPotentialIndexes();

    public Dictionary<string, KeyValuePair<ISelectionOperator, double>> CanGetPreSelected(Dictionary<string, string> tableNames);

    public double CalculateSelectivity(Dictionary<string, string> tableNames);

}