namespace EmployeeManagementGraphQL.Data.Models.Paging;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; }

    public PageInfo PageInfo { get; init; }

    public PagedResult(IReadOnlyList<T> items, PageInfo pageInfo)
    {
        Items = items;
        PageInfo = pageInfo;
    }
}
