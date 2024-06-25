namespace SchmidiDB.Storage.System;

public class Statistics(string table, Dictionary<string, int> numOfDistinctValues, Dictionary<string, Dictionary<object, double>> mostCommonValues, Dictionary<string, double> nullFraction)
{

    public string Table { get; } = table;

    public Dictionary<string, int> NumOfDistinctValues { get; } = numOfDistinctValues;

    public Dictionary<string, Dictionary<object, double>> MostCommonValues { get; } = mostCommonValues;

    public Dictionary<string, double> NullFraction { get; } = nullFraction;

}