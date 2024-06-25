using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using SchmidiDB.Query.Access;
using SchmidiDB.Storage;
using SchmidiDB.Storage.Index;
using SchmidiDB.TestDatabases;
using SchmidiDB.Tests.Util;

namespace SchmidiDB.Tests;

[SimpleJob(RuntimeMoniker.Net80)]
[RPlotExporter]
public class AccessBenchmark
{
    // public List<string> Files =>
    // [
    //     "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark10max.xml",
    //     "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark90max.xml"
    //
    // ];
    // [ParamsSource(nameof(Files))]
    // public string File;

    [ParamsSource(nameof(AccessOperators))]
    public string AccessOperator;

    public object accessOperator;

    public List<string> AccessOperators =>
    [
        "sequential",
        "hash"
    ];

    [ParamsSource(nameof(SystemCatalogs))]
    public FileWrapper SysCatalog;

    public IEnumerable<FileWrapper> SystemCatalogs()
    {
        List<string> names = [
            "10% 1,000 rows",
            "25% 1,000 rows",
            "50% 1,000 rows",
            "75% 1,000 rows",
            "90% 1,000 rows",
            "100% 1,000 rows",
            "10% 100,000 rows",
            "25% 100,000 rows",
            "50% 100,000 rows",
            "75% 100,000 rows",
            "90% 100,000 rows",
            "100% 100,000 rows",
            "10% 1,000,000 rows",
            "25% 1,000,000 rows",
            "50% 1,000,000 rows",
            "75% 1,000,000 rows",
            "90% 1,000,000 rows",
            "100% 1,000,000 rows",

        ];
        List<string> files =
        [
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark10min.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark25min.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark50min.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark75min.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark90min.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmarkmin.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark10.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark25.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark50.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark75.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark90.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark10max.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark25max.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark50max.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark75max.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmark90max.xml",
            "D:\\\\FH\\\\Master\\\\Masterarbeit\\\\The Real Shit\\\\schmidiDB\\\\SchmidiDB\\\\SchmidiDB\\\\TestDatabases\\\\indexBenchmarkmax.xml",
        ];
        for (int i = 0; i < names.Count; i++)
        {
            SystemCatalog.Delete();
            TestDatabase.UseTestDatabase(files[i]);
            SystemCatalog.Instance.CreateIndex("attr", "tests");
            yield return new FileWrapper(names[i], SystemCatalog.Instance);
        }
    }

    // [IterationSetup]
    // public void IterationSetup()
    // {
    //     SystemCatalog.Delete();
    //     TestDatabase.UseTestDatabase(File);
    //     SystemCatalog.Instance.CreateIndex("attr", "tests");    
    //     if (AccessOperator == "sequential")
    //     {
    //         accessOperator = new SequentialScanAccessOperator();
    //     }
    //     else
    //     {
    //         accessOperator = SystemCatalog.Instance.GetIndex("tests", "attr");
    //     }
    // }
    
    
    [Benchmark]
    public List<Row> Execute()
    {
        // var hashTable = accessOperator as HashTableIndex;
        // var sequential = accessOperator as SequentialScanAccessOperator;
        if (AccessOperator == "hash")
        {
            return SysCatalog.SysCatalog.GetIndex("tests", "attr").Scan(42, row => row["attr"].Equals(42)).ToList();
        }
        else
        {
            return new SequentialScanAccessOperator().Scan(SysCatalog.SysCatalog.Tables["tests"], row => row["attr"].Equals(42)).ToList();
        }
    }
}