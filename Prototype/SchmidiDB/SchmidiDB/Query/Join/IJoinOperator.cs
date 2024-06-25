using SchmidiDB.Storage;
using OneOf;
using SchmidiDB.Query.Join.FromClause;

namespace SchmidiDB.Query.Join;

public interface IJoinOperator
{
    public IEnumerable<Row> Join(IEnumerable<Row> leftRelation, string? leftRelationName, IEnumerable<Row> rightRelation, string? rightRelationName, string leftJoinKey,
        string rightJoinKey, List<string> orderedLeft, List<string> orderedRight, Row leftNullRow, Row rightNullRow, JoinType joinType = JoinType.InnerJoin);

    public double EstimateCosts(long estimatedLeft, long estimatedRight, long estimatedResult, FromClause.FromClause from, JoinDeclaration joinDeclaration, List<string> orderedLeft, List<string> orderedRight);

    public List<string> GetOrderedByAfterJoin(List<string> orderLeft, List<string> orderRight, JoinDeclaration joinDeclaration);


    public List<string> GetOrderedByAfterJoin(List<string> orderLeft, List<string> orderRight,
        string nameLeft, string nameRight, string keyLeft, string keyRight, JoinType joinType)
    {
        return GetOrderedByAfterJoin(orderLeft, orderRight,
            new JoinDeclaration(nameLeft, nameRight, keyLeft, keyRight, joinType));
    }
}