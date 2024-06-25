using SchmidiDB.Query.Join.Tree;

namespace SchmidiDB.Tests.Util;

public class JoinTreeWrapper(double costs, long rows, JoinTree joinTree)
{
    public double Costs { get; set; } = costs;

    public long Rows { get; set; } = rows;

    public JoinTree JoinTree { get; set; } = joinTree;

    public override string ToString()
    {
        return JoinTree.ShortDescription();
    }
}