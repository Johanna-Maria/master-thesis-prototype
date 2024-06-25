using SchmidiDB.Storage;

namespace SchmidiDB.Tests.Util;

public class FileWrapper(string name, SystemCatalog systemCatalog)
{
    public SystemCatalog SysCatalog { get; set; } = systemCatalog;

    public string Name { get; set; } = name;

    public override string ToString()
    {
        return Name;
    }
}