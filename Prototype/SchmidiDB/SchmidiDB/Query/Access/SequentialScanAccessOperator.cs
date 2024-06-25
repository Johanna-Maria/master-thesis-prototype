using SchmidiDB.Storage;

namespace SchmidiDB.Query.Access;

public class SequentialScanAccessOperator
{
    public IEnumerable<Row> Scan(IEnumerable<Row> table, Func<Row, bool> predicate)
    {
        foreach (Row row in table)
        {
            if (predicate(row))
            {
                yield return row;
            }
        }
    }
}