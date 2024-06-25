using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection;

public class OrSelectionOperator(ISelectionOperator left, ISelectionOperator right) : ISelectionOperator
{
    
    private ISelectionOperator Left { get; set; } = left;

    private ISelectionOperator Right { get; set; } = right;
    public bool Calculate(Row row)
    {
        return Left.Calculate(row) || Right.Calculate(row);
    }

    public double CalculateSelectivity(Dictionary<string, string> tableNames)
    {
        var leftSelectivity = Left.CalculateSelectivity(tableNames);
        var rightSelectivity = Right.CalculateSelectivity(tableNames);
        return leftSelectivity + rightSelectivity - leftSelectivity * rightSelectivity;
    }

    public IEnumerable<KeyValuePair<string, object?>> GetPotentialIndexes()
    {
        return new List<KeyValuePair<string, object?>>();
    }

    public Dictionary<string, KeyValuePair<ISelectionOperator, double>> CanGetPreSelected(Dictionary<string, string> tableNames)
    {
        var leftTrees = Left.CanGetPreSelected(tableNames);
        var rightTrees = Right.CanGetPreSelected(tableNames);
        if (leftTrees.Count == 0 && rightTrees.Count == 0) return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>();
        if (leftTrees.Count == 0 && rightTrees.Count != 0) return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>();
        if (leftTrees.Count != 0 && rightTrees.Count == 0) return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>();
        // var result = (Dictionary<string, KeyValuePair<ISelectionOperator, double>>) leftTrees.Union(rightTrees).GroupBy(pair => pair.Key).SelectMany(group => {
        //     if (group.Count() > 1)
        //     {
        //         ISelectionOperator leftSide = group.ToArray()[0].Value.Key;
        //         ISelectionOperator rightSide = group.ToArray()[1].Value.Key;
        //         double leftSelectivity = group.ToArray()[0].Value.Value;
        //         double rightSelectivity = group.ToArray()[1].Value.Value;
        //         return new List<KeyValuePair<string, KeyValuePair<ISelectionOperator, double>?>> {new (group.Key,
        //             new (
        //                 new OrSelectionOperator(leftSide, rightSide),
        //                 leftSelectivity + rightSelectivity - leftSelectivity * rightSelectivity
        //                 )
        //             )};
        //     }
        //
        //     return new List<KeyValuePair<string, KeyValuePair<ISelectionOperator, double>?>>{new (group.Key, null)}; //group.Select(e => e);
        // }).Where(pair => pair.Value is not null).ToDictionary();
        var result = leftTrees.Union(rightTrees).GroupBy(pair => pair.Key).SelectMany(group => {
                if (group.Count() > 1)
                {
                    ISelectionOperator leftSide = group.ToArray()[0].Value.Key;
                    ISelectionOperator rightSide = group.ToArray()[1].Value.Key;
                    double leftSelectivity = group.ToArray()[0].Value.Value;
                    double rightSelectivity = group.ToArray()[1].Value.Value;
                    return new List<KeyValuePair<string, KeyValuePair<ISelectionOperator, double?>>> {new (group.Key,
                        new (
                            new OrSelectionOperator(leftSide, rightSide),
                            leftSelectivity + rightSelectivity - leftSelectivity * rightSelectivity
                        )
                    )};
                }
        
                return new List<KeyValuePair<string, KeyValuePair<ISelectionOperator, double?>>>{new (group.Key, new KeyValuePair<ISelectionOperator, double?>(this, null))}; //group.Select(e => e);
            }).Where(pair => pair.Value.Value is not null)
            .Select(pair => new KeyValuePair<string,KeyValuePair<ISelectionOperator,double>>(pair.Key,
                new KeyValuePair<ISelectionOperator,double>(pair.Value.Key, (double) pair.Value.Value)) ).ToDictionary();
        return result;
    }

    public override string ToString()
    {
        return "(" + Left.ToString() + " OR " + Right.ToString() + ")";
    }
}