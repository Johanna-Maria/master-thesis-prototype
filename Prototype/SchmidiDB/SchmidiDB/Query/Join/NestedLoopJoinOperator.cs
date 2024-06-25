using SchmidiDB.Storage;
using SchmidiDB.Storage.Index;
using SchmidiDB.Storage.System;
using OneOf;
using SchmidiDB.Query.Join.FromClause;

namespace SchmidiDB.Query.Join;

public class NestedLoopJoinOperator : IJoinOperator
{
    public IEnumerable<Row> Join(IEnumerable<Row> leftRelation, string? leftRelationName, IEnumerable<Row> rightRelation, string? rightRelationName,
        string leftJoinKey, string rightJoinKey, List<string> orderedLeft, List<string> orderedRight, Row leftNullRow, Row rightNullRow, JoinType joinType = JoinType.InnerJoin)
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
        // var rightNullRow = new Row();
        // if (joinType is JoinType.LeftOuterJoin)
        // {
        //     rightNullRow = rightRelation.First().GenerateNullRow();
        // }
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
            foreach (Row rightRow in rightRelation)
            {
                if (leftRow[leftJoinKey] is not null && rightRow[rightJoinKey] is not null && leftRow[leftJoinKey].Equals(rightRow[rightJoinKey]) )
                {
                    joined = true;
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
        }
        var systemCatalog = SystemCatalog.Instance;

        return estimatedLeft * 0.2 + estimatedLeft * estimatedRight * 0.2;;
    }
    
    public List<string> GetOrderedByAfterJoin(List<string> orderLeft, List<string> orderRight, JoinDeclaration joinDeclaration)
    {
        if (joinDeclaration.JoinType is not JoinType.RightOuterJoin) return orderLeft;
        return orderRight; //because tables get swapped
    }
    
    public override string ToString()
    {
        return "Nested-Loop Join";
    }
}