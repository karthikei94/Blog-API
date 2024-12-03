namespace BlogApp.Models;

public class PagedResult<T> where T : class
{
    public long TotalRec { get; set; } = 0;
    public List<T> Records { get; set; } = [];
}