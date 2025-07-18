namespace SurveryBasket.Api.Abstractions;

public class PaginatedList<T>
{
    private PaginatedList(List<T> items , int pageNumber, int pageSize , int count)
    {
        Items = items ;
        PageNumber = pageNumber ;
        TotalPages = (int)Math.Ceiling(count/(double)pageSize);
    }
    public List<T> Items { get; init; }
    public int PageNumber { get; init; }
    public int TotalPages { get; init; }
    public bool pervoiusPage
        => PageNumber > 1;
    public bool NextPage
        => PageNumber < TotalPages;
    public static async Task<PaginatedList<T>> CreatePaginatedList(IQueryable<T> source , int pageNumber , int pageSize , CancellationToken cancellation)
    {
        var count = await source.CountAsync(cancellation);
        var items = await source.Skip((pageNumber-1)*pageSize).Take(pageSize).ToListAsync(cancellation);
        return new PaginatedList<T>(items, pageNumber, pageSize, count);
    }
}
