using OneOf;
using SchmidiDB.Query.Join.FromClause;
using SchmidiDB.Storage;
using SchmidiDB.Storage.Index;

namespace SchmidiDB.Query.Join;

public class NestedLoopJoinWithIndexOperator : IJoinOperator
{
    public IEnumerable<Row> Join(IEnumerable<Row> leftRelation, string? leftRelationName, IEnumerable<Row> rightRelation, string? rightRelationName,
        string leftJoinKey, string rightJoinKey, List<string> orderedLeft, List<string> orderedRight,  Row leftNullRow, Row rightNullRow, JoinType joinType = JoinType.InnerJoin)
    {

        if (joinType == JoinType.RightOuterJoin)
        {
            //Switch tables and use a left outer join
            var relationTmp = leftRelation;
            var nameTmp = leftRelationName;
            var keyTmp = leftJoinKey;
            leftRelation = rightRelation;
            leftRelationName = rightRelationName;
            leftJoinKey = rightJoinKey;
            rightRelation = relationTmp;
            rightRelationName = nameTmp;
            rightJoinKey = keyTmp;
            joinType = JoinType.LeftOuterJoin;
        }
        if (joinType is JoinType.OuterJoin)
        {
            //https://www.ibm.com/docs/en/doefz/2.1?topic=constructs-nested-loop-join-construct
            //https://learn.microsoft.com/de-de/archive/blogs/craigfr/nested-loops-join
            throw new Exception("Cannot use Outer Join in a Nested Loop Join");
        }


        HashTableIndex? index = null;
        if (rightRelation is Table && SystemCatalog.Instance.GetIndex(((Table) rightRelation).Name, rightJoinKey) != null )
        {
            index = SystemCatalog.Instance.GetIndex(((Table) rightRelation).Name, rightJoinKey);
        }
        if (index is null)
        {
            throw new Exception("There is no index for Nested Loop join with index");
        }
        foreach (Row leftRow in leftRelation)
        {
            if (leftRow[leftJoinKey] is null)
            {
                if (joinType == JoinType.LeftOuterJoin)
                {
                    yield return new Row(leftRow, rightNullRow, leftRelationName, rightRelationName);

                }
                continue;
            }
            bool joined = false;
            var rightRows = index.Scan(leftRow[leftJoinKey]).ToList();
            if (rightRows.Any())
            {
                joined = true;
                foreach (var rightRow in rightRows)
                {
                    yield return new Row(leftRow, rightRow, leftRelationName, rightRelationName);
                }
            }
            if (!joined && joinType is JoinType.LeftOuterJoin)
            {
                yield return new Row(leftRow, rightNullRow, leftRelationName, rightRelationName);
            }
        }
    }


    public double EstimateCosts(long estimatedLeft, long estimatedRight, long estimatedResult, FromClause.FromClause from, JoinDeclaration joinDeclaration, List<string> orderedLeft, List<string> orderedRight)
    {
        if (joinDeclaration.JoinType == JoinType.RightOuterJoin)
        {
            return EstimateCosts(estimatedRight, estimatedLeft,
                estimatedResult,
                from, 
                new JoinDeclaration(joinDeclaration.RightRelation, joinDeclaration.LeftRelation, joinDeclaration.RightJoinKey, joinDeclaration.LeftJoinKey, JoinType.LeftOuterJoin),
                orderedRight,
                orderedLeft);
        }

        if (joinDeclaration.JoinType == JoinType.OuterJoin)
        {
            return double.MaxValue;
        };
        var systemCatalog = SystemCatalog.Instance;
        if (from.Data[joinDeclaration.RightRelation] is {IsT0: true, AsT0: { } rightTable} && systemCatalog.GetIndex(rightTable.Name, joinDeclaration.RightJoinKey) != null)
        {
            //use number of rows from the "real" table, not the preselection (the index works with the real table)
            var rightNumOfRows = systemCatalog.Tables[rightTable.Name].Count();
            var leftSelectivity =  from.Data[joinDeclaration.LeftRelation].IsT0 && from.Data[joinDeclaration.LeftRelation].AsT0.Count() == estimatedLeft ? 1.0 / systemCatalog.GetNumOfDistinctValuesFor(from.Data[joinDeclaration.LeftRelation].AsT0.Name, joinDeclaration.LeftJoinKey) : 0.1;
            var rightSelectivity =  from.Data[joinDeclaration.RightRelation].IsT0 ? 1.0 / systemCatalog.GetNumOfDistinctValuesFor(from.Data[joinDeclaration.RightRelation].AsT0.Name, joinDeclaration.RightJoinKey) : 0.1;
            estimatedResult = Convert.ToInt64(Math.Ceiling(estimatedLeft * rightNumOfRows * Double.Min(leftSelectivity, rightSelectivity)));
            return 0.2 * estimatedLeft + 2 * estimatedLeft * long.Max(estimatedResult/estimatedLeft, 1);
        }
        //there is no index
        return Double.MaxValue;
    }
    
    public List<string> GetOrderedByAfterJoin(List<string> orderLeft, List<string> orderRight, JoinDeclaration joinDeclaration)
    {
        if (joinDeclaration.JoinType is not JoinType.RightOuterJoin) return orderLeft;
        return orderRight; //because tables get swapped
    }
    
    public override string ToString()
    {
        return "Nested-Loop Join (Index)";
    }
}