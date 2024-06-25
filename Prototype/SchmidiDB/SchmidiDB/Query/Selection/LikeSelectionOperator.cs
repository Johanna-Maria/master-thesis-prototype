using System.Text.RegularExpressions;
using SchmidiDB.Query.Selection.Leaves;
using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection;

public class LikeSelectionOperator(SelectionLeaveColumn left, SelectionLeaveConstant right) : ISelectionOperator
{

    public SelectionLeaveColumn LeftChild { get; set; } = left;

    public SelectionLeaveConstant RightChild { get; set; } = right;
    public bool Calculate(Row row)
    {
        string? needle = LeftChild.Calculate(row) as string;
        string? haystack = RightChild.Value as string;
        if (haystack == null || needle == null) return false;
        haystack = Regex.Replace(haystack, "_", ".");
        haystack = Regex.Replace(haystack, "%", ".*");
        MatchCollection matches = Regex.Matches(needle, "^" + haystack+"$");
        return matches.Any();
    }

    public double CalculateSelectivity(Dictionary<string, string> tableNames)
    {
        return 0.1;
    }

    public IEnumerable<KeyValuePair<string, object?>> GetPotentialIndexes()
    {
        return new List<KeyValuePair<string, object?>>();
    }

    public Dictionary<string, KeyValuePair<ISelectionOperator, double>> CanGetPreSelected(Dictionary<string, string> tableNames)
    {
        var leftColumn = left.Name.Split(".");
        var tmp = leftColumn[^1];
        return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>
        {
            {
                tableNames[leftColumn[0]], 
                new KeyValuePair<ISelectionOperator, double>(
                    new LikeSelectionOperator(new SelectionLeaveColumn(leftColumn[^1]), RightChild), 0.1)
            }
        };
    }
    
    public override string ToString()
    {
        return LeftChild + " LIKE " + RightChild;
    }
}