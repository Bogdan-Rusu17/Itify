namespace Itify.DbService.Infrastructure;

public class PagedResponse<T>(int page, int pageSize, int totalCount, List<T> data)
{
    public int Page { get; set; } = page;
    public int PageSize { get; set; } = pageSize;
    public int TotalCount { get; set; } = totalCount;
    public List<T> Data { get; set; } = data;
}
