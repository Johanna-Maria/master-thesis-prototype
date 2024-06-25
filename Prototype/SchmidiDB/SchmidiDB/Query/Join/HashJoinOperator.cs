using SchmidiDB.Storage;
using SchmidiDB.Storage.System;
using OneOf;
using SchmidiDB.Query.Join.FromClause;

namespace SchmidiDB.Query.Join;

public class HashJoinOperator : IJoinOperator
{
    public IEnumerable<Row> Join(IEnumerable<Row> leftRelation, string? leftRelationName, IEnumerable<Row> rightRelation, string? rightRelationName,
        string leftJoinKey, string rightJoinKey, List<string> orderedLeft, List<string> orderedRight,  Row leftNullRow, Row rightNullRow, JoinType joinType = JoinType.InnerJoin)
    {

        var hashTable = new Dictionary<object, KeyValuePair<List<Row>, bool>>();
        foreach (var row in leftRelation)
        {
            if (row[leftJoinKey] == null)
            {
                if (joinType is JoinType.OuterJoin or JoinType.LeftOuterJoin)
                {
                    yield return new Row(row, rightNullRow, leftRelationName, rightRelationName);
                }
            }
            else if (hashTable.ContainsKey(row[leftJoinKey]))
            {
                hashTable[row[leftJoinKey]].Key.Add(row);
            }
            else
            {
                hashTable[row[leftJoinKey]] = new KeyValuePair<List<Row>, bool>(new List<Row>(), false);
                hashTable[row[leftJoinKey]].Key.Add(row);
            }
        }

        foreach (var row in rightRelation)
        {
            if (row[rightJoinKey] == null)
            {
                if (joinType is JoinType.OuterJoin or JoinType.RightOuterJoin)
                {
                    yield return new Row(leftNullRow, row, leftRelationName, rightRelationName);
                }
            }
            else if (hashTable.ContainsKey(row[rightJoinKey]))
            {
                if (!hashTable[row[rightJoinKey]].Value && joinType is JoinType.OuterJoin or JoinType.LeftOuterJoin)
                {
                    hashTable[row[rightJoinKey]] =
                        new KeyValuePair<List<Row>, bool>(hashTable[row[rightJoinKey]].Key, true);
                }
                foreach (var leftRow in hashTable[row[rightJoinKey]].Key)
                {
                    yield return new Row(leftRow, row, leftRelationName, rightRelationName);
                }
            }
            else if (joinType is JoinType.OuterJoin or JoinType.RightOuterJoin)
            {
                yield return new Row(leftNullRow, row, leftRelationName, rightRelationName);
            }
        }

        if (joinType is JoinType.OuterJoin or JoinType.LeftOuterJoin)
        {
            foreach (var leftRow in hashTable.Where(pairPair => !pairPair.Value.Value).SelectMany(pairPair => pairPair.Value.Key))
            {
                yield return new Row(leftRow, rightNullRow, leftRelationName, rightRelationName);
        
            }
        }
    }

    public double EstimateCosts(long estimatedLeft, long estimatedRight, long estimatedResult, FromClause.FromClause from, JoinDeclaration joinDeclaration, List<string> orderedLeft, List<string> orderedRight)
    {
        return estimatedResult + estimatedLeft + 0.2 * estimatedLeft + 0.2 * estimatedRight;
    }
    
    public List<string> GetOrderedByAfterJoin(List<string> orderLeft, List<string> orderRight, JoinDeclaration joinDeclaration)
    {
        if (joinDeclaration.JoinType is JoinType.InnerJoin or JoinType.RightOuterJoin) return orderRight;
        return [];
    }
    
    public override string ToString()
    {
        return "Hash Join";
    }
}