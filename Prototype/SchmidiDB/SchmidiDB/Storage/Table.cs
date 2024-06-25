using System.Collections;

namespace SchmidiDB.Storage;
public class Table(String name, IEnumerable<Row> data) : IEnumerable<Row>
{
    public String Name { get; set; } = name;

    public List<Row> Data { get; set; } = data.ToList();

    public void AddRow(Row row)
    {
        Data.Add(row);
    }
    
    public IEnumerator<Row> GetEnumerator()
    {
        return data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}