namespace EmployeeManagementGraphQL.Data.Models.Paging;

public sealed class PaginationRequest
{
    private const int DefaultPage = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    public int Page { get; }

    public int PageSize { get; }

    private PaginationRequest(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public static PaginationRequest Normalize(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? DefaultPage : page;
        var normalizedPageSize = pageSize < 1 ? DefaultPageSize : pageSize;

        if (normalizedPageSize > MaxPageSize)
        {
            normalizedPageSize = MaxPageSize;
        }

        return new PaginationRequest(normalizedPage, normalizedPageSize);
    }
}
