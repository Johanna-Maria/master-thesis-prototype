using System.Diagnostics;
using SchmidiDB.Storage;
using OneOf;
using SchmidiDB.Query.Join.FromClause;
using SchmidiDB.Storage.System;

namespace SchmidiDB.Query.Join;

public class SortMergeJoinOperator : IJoinOperator
{
    public IEnumerable<Row> Join(IEnumerable<Row> leftRelation, string? leftRelationName, IEnumerable<Row> rightRelation, string? rightRelationName,
        string leftJoinKey, string rightJoinKey, List<string> orderedLeft, List<string> orderedRight, Row leftNullRow, Row rightNullRow, JoinType joinType = JoinType.InnerJoin)
    {
        var sortedLeftRelation = (orderedLeft.Contains(leftRelationName+"."+leftJoinKey) ? leftRelation : leftRelation.OrderBy(row => row[leftJoinKey])).ToList();
        var sortedRightRelation = (orderedRight.Contains(rightRelationName+"."+rightJoinKey) ? rightRelation : rightRelation.OrderBy(row => row[rightJoinKey])).ToList();
        int left = 0;
        int right = 0;
        object? lastMatch = null;
        bool notMatchedYet = true;
        //https://learn.microsoft.com/de-de/archive/blogs/craigfr/merge-join
        while (left < sortedLeftRelation.Count && sortedLeftRelation[left][leftJoinKey] is null)
        {
            if (joinType is JoinType.LeftOuterJoin or JoinType.OuterJoin)
            {
                yield return new Row(sortedLeftRelation[left], rightNullRow, leftRelationName, rightRelationName);
            }
            left++;
        }
        while (right < sortedRightRelation.Count && sortedRightRelation[right][rightJoinKey] is null)
        {
            if (joinType is JoinType.RightOuterJoin or JoinType.OuterJoin)
            {
                yield return new Row(leftNullRow, sortedRightRelation[right], leftRelationName, rightRelationName);
            }
            right++;
        }
        while (left < sortedLeftRelation.Count && right < sortedRightRelation.Count)
        {
            if (sortedLeftRelation[left][leftJoinKey] is not null 
                && sortedRightRelation[right][rightJoinKey] is not null 
                && sortedLeftRelation[left][leftJoinKey].Equals(sortedRightRelation[right][rightJoinKey]))
            {
                yield return new Row(sortedLeftRelation[left], sortedRightRelation[right], leftRelationName,
                    rightRelationName);
                int i = left + 1;
                while (i < sortedLeftRelation.Count 
                       && sortedLeftRelation[i][leftJoinKey] is not null 
                       && sortedLeftRelation[i][leftJoinKey].Equals(sortedRightRelation[right][rightJoinKey]))
                {
                    yield return new Row(sortedLeftRelation[i], sortedRightRelation[right], leftRelationName,
                        rightRelationName);
                    i++;
                }

                i = right + 1;
                while (i < sortedRightRelation.Count 
                       && sortedRightRelation[i][rightJoinKey] is not null 
                       && sortedRightRelation[i][rightJoinKey].Equals(sortedLeftRelation[left][leftJoinKey]))
                {
                    yield return new Row(sortedLeftRelation[left], sortedRightRelation[i], leftRelationName,
                        rightRelationName);
                    i++;
                }

                notMatchedYet = false;
                lastMatch = sortedLeftRelation[left][leftJoinKey];
                
                right++;
                left++;
            }
            else if (IsLessThan(sortedLeftRelation[left][leftJoinKey], sortedRightRelation[right][rightJoinKey]))
            {
                if (joinType is JoinType.OuterJoin or JoinType.LeftOuterJoin && (notMatchedYet 
                        || sortedLeftRelation[left][leftJoinKey] is null 
                        || !lastMatch.Equals(sortedLeftRelation[left][leftJoinKey])))
                {
                    yield return new Row(sortedLeftRelation[left], rightNullRow, leftRelationName, rightRelationName);
                }
                left++;
            }
            else
            {
                if (joinType is JoinType.OuterJoin or JoinType.RightOuterJoin && (notMatchedYet 
                        || sortedRightRelation[right][rightJoinKey] is null 
                        || !lastMatch.Equals(sortedRightRelation[right][rightJoinKey])))
                {
                    yield return new Row(leftNullRow, sortedRightRelation[right], leftRelationName, rightRelationName);
                }
                right++;
            }
        }

        while (joinType is JoinType.OuterJoin or JoinType.LeftOuterJoin && left < sortedLeftRelation.Count)
        {
            if (!sortedLeftRelation[left][leftJoinKey].Equals(lastMatch))
            {
                yield return new Row(sortedLeftRelation[left], rightNullRow, leftRelationName, rightRelationName);
            }
            left++;
        }
        while (joinType is JoinType.OuterJoin or JoinType.RightOuterJoin && right < sortedRightRelation.Count)
        {
            if (!sortedRightRelation[right][rightJoinKey].Equals(lastMatch))
            {
                yield return new Row(leftNullRow, sortedRightRelation[right], leftRelationName, rightRelationName);
            }
            right++;
        }
    }

    private bool IsLessThan(object? left, object? right)
    {
        if (left == null)
        {
            return right != null;
        }
        if (left is int l && right is int r)
        {
            return l < r;
        }
        return String.CompareOrdinal((string) left, (string) right) < 0;
    }
    
    public double EstimateCosts(long estimatedLeft, long estimatedRight, long estimatedResult, FromClause.FromClause from, JoinDeclaration joinDeclaration, List<string> orderedLeft, List<string> orderedRight)
    {
        var systemCatalog = SystemCatalog.Instance;
        var leftCosts = estimatedLeft * 1.0;
        var rightCosts = estimatedRight * 1.0;
        leftCosts = orderedLeft.Contains(joinDeclaration.LeftRelation+"."+joinDeclaration.LeftJoinKey)
            ? 0.2 * leftCosts
            : 0.2 * leftCosts + leftCosts;
        rightCosts = orderedRight.Contains(joinDeclaration.RightRelation+"."+joinDeclaration.RightJoinKey)
            ? 0.2 * rightCosts
            : 0.2 * rightCosts + rightCosts;
        return leftCosts + rightCosts + estimatedResult;
    }

    public List<string> GetOrderedByAfterJoin(List<string> orderLeft, List<string> orderRight, JoinDeclaration joinDeclaration)
    {
        return [joinDeclaration.LeftRelation+"."+joinDeclaration.LeftJoinKey, joinDeclaration.RightRelation+"."+joinDeclaration.RightJoinKey];
    }

    public override string ToString()
    {
        return "Sort-Merge Join";
    }
}