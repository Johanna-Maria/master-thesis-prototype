namespace SchmidiDB.Storage.System;

public static class SystemParameters
{
    public static double DefaultJoinSelectivity { get; } = 0.10;

    public static int MostCommonValuesThreshold { get; } = 100;

    public static double IndexScanThreshold { get; } = 1.0;
}