namespace FTSBenchmark.Models;

public class ListResponse<T>
{
    private ListResponse(List<T> items)
    {
        Items = items;
    }
    
    public int Count => Items.Count;
    
    public List<T> Items { get; }
    
    public static ListResponse<T> FromData(IEnumerable<T> items)
    {
        return new ListResponse<T>(items.ToList());
    }
}