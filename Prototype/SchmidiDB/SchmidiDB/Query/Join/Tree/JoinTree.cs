using System.Collections;
using OneOf;
using SchmidiDB.Query.Access;
using SchmidiDB.Storage;

namespace SchmidiDB.Query.Join.Tree;

public class JoinTree(FromClause.FromClause fromClause,
    OneOf<JoinTree, Table, Query> left,
    OneOf<JoinTree, Table, Query> right,
    JoinType joinType,
    string leftJoinColumn,
    string rightJoinColumn,
    IJoinOperator joinOperator,
    string? leftRelationName,
    string? rightRelationName,
    List<string> leftOrderedBy,
    List<string> rightOrderedBy
    )
{
    public OneOf<JoinTree, Table, Query> Left { get; set; } = left;

    public OneOf<JoinTree, Table, Query> Right { get; set; } = right;

    public JoinType JoinType { get; set; } = joinType;

    public IJoinOperator JoinOperator { get; set; } = joinOperator;
    
    public string LeftJoinColumn { get; set; } = leftJoinColumn;

    public string RightJoinColumn { get; set; } = rightJoinColumn;

    public string? LeftRelationName { get; set; } = leftRelationName;

    public string? RightRelationName { get; set; } = rightRelationName;

    public List<string> LeftOrderedBy { get; set; } = leftOrderedBy;
    
    public List<string> RightOrderedBy { get; set; } = rightOrderedBy;


    public IEnumerable<Row> Execute()
    {
        //getting the new Table from the FromClause, as the data might have changed due to Preselection
        IEnumerable<Row> leftRelation = (IEnumerable<Row>) Left.MapT0(tree => tree.Execute()).MapT1(table => fromClause.Data[LeftRelationName].AsT0).MapT2(query => query.Execute(false)).Value;
        IEnumerable<Row> rightRelation = (IEnumerable<Row>) Right.MapT0(tree => tree.Execute()).MapT1(table => fromClause.Data[RightRelationName].AsT0).MapT2(query => query.Execute(false)).Value;
        return JoinOperator.Join(leftRelation, LeftRelationName, rightRelation, RightRelationName, LeftJoinColumn,
            RightJoinColumn, LeftOrderedBy, RightOrderedBy, LeftNullRow(), RightNullRow(), JoinType);
    }

    private Row LeftNullRow()
    {
        return (Row) Left.MapT0(tree => tree.LeftNullRow())
            .MapT1(table => new Row(SystemCatalog.Instance.TableInfos[table.Name].Columns
                .Select(column => new KeyValuePair<string, object?>(column, null))))
            .MapT2(query => new Row(query.Projection.Select(column => new KeyValuePair<string, object?>(column, null)))).Value;
    }
    
    private Row RightNullRow()
    {
        return (Row) Right.MapT0(tree => tree.LeftNullRow())
            .MapT1(table => new Row(SystemCatalog.Instance.TableInfos[table.Name].Columns
                .Select(column => new KeyValuePair<string, object?>(column, null))))
            .MapT2(query => new Row(query.Projection.Select(column => new KeyValuePair<string, object?>(column, null)))).Value;
    }

    public override string ToString()
    {
        Dictionary<JoinType, string> joinTypes = new Dictionary<JoinType, string>
        {
            {JoinType.InnerJoin, "INNER JOIN"},
            {JoinType.OuterJoin, "OUTER JOIN"},
            {JoinType.LeftOuterJoin, "LEFT OUTER JOIN"},
            {JoinType.RightOuterJoin, "RIGHT OUTER JOIN"},

        };
        string result = " " + (string) Left.MapT1(table => ((Table) table).Name).MapT0(tree => tree.ToString()).MapT2(query => "( " + query + " )")
            .Value;
        result += " " + (leftRelationName ?? "") + " " + joinTypes[joinType] + " ";
        result += (string) Right.MapT1(table => ((Table) table).Name).MapT0(tree => tree.ToString()).MapT2(query => "( " + query + " )")
            .Value;
        result += " " + (rightRelationName ?? "");
        result += " ON (" + (leftRelationName != null ? leftRelationName + "." : "") + leftJoinColumn + " = " + (rightRelationName != null ? rightRelationName + "." : "") + rightJoinColumn + ")";
        return result;
    }
    
    public string PrintJoinTree()
    {
        Dictionary<JoinType, string> joinTypes = new Dictionary<JoinType, string>
        {
            {JoinType.InnerJoin, "INNER JOIN"},
            {JoinType.OuterJoin, "OUTER JOIN"},
            {JoinType.LeftOuterJoin, "LEFT OUTER JOIN"},
            {JoinType.RightOuterJoin, "RIGHT OUTER JOIN"},

        };
        string result = " " + (string) Left.MapT1(table => ((Table) table).Name).MapT0(tree => tree.PrintJoinTree()).MapT2(query => "( " + query + " )")
            .Value;
        result += " " + (leftRelationName ?? "") + " " + joinTypes[joinType] + " [" + joinOperator.GetType() + " ] ";
        result += (string) Right.MapT1(table => ((Table) table).Name).MapT0(tree => tree.PrintJoinTree()).MapT2(query => "( " + query + " )")
            .Value;
        result += " " + (rightRelationName ?? "");
        result += " ON (" + (leftRelationName != null ? leftRelationName + "." : "") + leftJoinColumn + " = " + (rightRelationName != null ? rightRelationName + "." : "") + rightJoinColumn + ")";
        return result;
    }

    public string ShortDescription()
    {
        return $"{(string) Left.MapT2(query => LeftRelationName).MapT1(table => LeftRelationName).MapT0(tree => tree.ShortDescription()).Value}, {(string) Right.MapT2(query => RightRelationName).MapT1(table => RightRelationName).MapT0(tree => tree.ShortDescription()).Value}";
    }

    public string ShortDescriptionJoinTypes()
    {
        var joinOpShort = new Dictionary<string, string>
        {
            [new NestedLoopJoinOperator().ToString()] = "NLJ",
            [new NestedLoopJoinWithIndexOperator().ToString()] = "NLIJ",
            [new SortMergeJoinOperator().ToString()] = "SMJ",
            [new HashJoinOperator().ToString()] = "HJ",

        };
        return
            $"{(string) Left.MapT0(tree => tree.ShortDescriptionJoinTypes()).MapT1(t => "").MapT2(q => "").Value} {joinOpShort[joinOperator.ToString()]}";
    }
}