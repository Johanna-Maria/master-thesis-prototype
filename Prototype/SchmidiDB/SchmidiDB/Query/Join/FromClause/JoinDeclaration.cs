namespace SchmidiDB.Query.Join.FromClause;

public class JoinDeclaration(string leftRelation, string rightRelation, string leftJoinKey, string rightJoinKey, JoinType joinType)
{
    public string LeftRelation
    {
        get => leftRelation;
        set => leftRelation = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string RightRelation
    {
        get => rightRelation;
        set => rightRelation = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string LeftJoinKey
    {
        get => leftJoinKey;
        set => leftJoinKey = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string RightJoinKey
    {
        get => rightJoinKey;
        set => rightJoinKey = value ?? throw new ArgumentNullException(nameof(value));
    }

    public JoinType JoinType
    {
        get => joinType;
        set => joinType = value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not JoinDeclaration that)
        {
            return false;
        }
        return LeftRelation == that.LeftRelation
               && RightRelation == that.RightRelation
               && LeftJoinKey == that.LeftJoinKey
               && RightRelation == that.RightRelation
               && JoinType == that.JoinType;
    }
}