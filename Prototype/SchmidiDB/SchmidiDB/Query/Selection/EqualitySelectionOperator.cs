using System.Runtime.CompilerServices;
using SchmidiDB.Query.Selection.Leaves;
using SchmidiDB.Storage;

namespace SchmidiDB.Query.Selection;

public class EqualitySelectionOperator(ISelectionLeave left, ISelectionLeave right) : ISelectionOperator
{
    public ISelectionLeave LeftChild { get; set; } = left;

    public ISelectionLeave RightChild { get; set; } = right;

    public bool Calculate(Row row)
    {
        var leftVal = LeftChild.Calculate(row);
        var rightVal = RightChild.Calculate(row);
        return leftVal != null && rightVal != null && leftVal.Equals(rightVal);
    }
    
    public double CalculateSelectivity(Dictionary<string, string> tableNames)
    {
        var systemCatalog = SystemCatalog.Instance;
        if (LeftChild is SelectionLeaveColumn leftCol && RightChild is SelectionLeaveColumn rightCol)
        {
            var leftColumn = leftCol.Name.Split(".");
            var rightColumn = rightCol.Name.Split(".");
            return 1.0 / Double.Max(systemCatalog.GetNumOfDistinctValuesFor(tableNames[leftColumn[0]], leftColumn[^1]), 
                systemCatalog.GetNumOfDistinctValuesFor(tableNames[rightColumn[0]], rightColumn[^1]));
        }
        if (LeftChild is SelectionLeaveConstant leftValue && RightChild is SelectionLeaveConstant rightValue)
        {
            return leftValue.Value == rightValue.Value ? 1.0 : 0.0;
        }
        var value = LeftChild is SelectionLeaveConstant
            ? LeftChild as SelectionLeaveConstant
            : RightChild as SelectionLeaveConstant;
        var column = LeftChild is SelectionLeaveColumn
            ? LeftChild as SelectionLeaveColumn
            : RightChild as SelectionLeaveColumn;
        var columnName = column.Name.Split(".");
        if (!tableNames.ContainsKey(columnName[0])) return 0.1; //subquery
        return systemCatalog.GetSelectivity(tableNames[columnName[0]], columnName[^1], value.Value);
    }
    
    public IEnumerable<KeyValuePair<string, object?>> GetPotentialIndexes()
    {
        var result = new List<KeyValuePair<string, object?>>();
        if (LeftChild is SelectionLeaveConstant && RightChild is not SelectionLeaveConstant)
        {
            result.Add(new KeyValuePair<string, object?>(((SelectionLeaveColumn) RightChild).Name, ((SelectionLeaveConstant)LeftChild).Value));
        } else if (LeftChild is not SelectionLeaveConstant && RightChild is SelectionLeaveConstant)
        {
            result.Add(new KeyValuePair<string, object?>(((SelectionLeaveColumn) LeftChild).Name, ((SelectionLeaveConstant)RightChild).Value));
        }

        return result;
    }

    public Dictionary<string, KeyValuePair<ISelectionOperator, double>> CanGetPreSelected(Dictionary<string, string> tableNames)
    {
        var systemCatalog = SystemCatalog.Instance;
        if (LeftChild is SelectionLeaveColumn && RightChild is SelectionLeaveColumn) return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>();
        //TODO check if column is column of a table and not of a query
        if (LeftChild is SelectionLeaveColumn left && tableNames.ContainsKey(left.Name.Split(".")[0]))
        {
            var leftColumn = left.Name.Split(".");
            return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>
            {
                {
                    tableNames[leftColumn[0]], new(
                        new EqualitySelectionOperator(new SelectionLeaveColumn(leftColumn[^1]), RightChild),
                        systemCatalog.GetSelectivity(tableNames[leftColumn[0]], leftColumn[^1], ((SelectionLeaveConstant )RightChild).Value)
                        )
                    
                }
            };
        }

        if (RightChild is SelectionLeaveColumn right && tableNames.ContainsKey(right.Name.Split(".")[0]))
        {
            var rightColumn = right.Name.Split(".");
            return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>
            {
                {
                    tableNames[rightColumn[0]], new(
                        new EqualitySelectionOperator(LeftChild, new SelectionLeaveColumn(rightColumn[^1])),
                        systemCatalog.GetSelectivity(tableNames[rightColumn[0]], rightColumn[^1], ((SelectionLeaveConstant )LeftChild).Value)
                    )
                    
                }
            };

        }
        //eg. 1 = 2
        return new Dictionary<string, KeyValuePair<ISelectionOperator, double>>();
    }

    public override string ToString()
    {
        return LeftChild + " = " + RightChild;
    }
}