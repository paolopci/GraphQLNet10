using EmployeeManagementGraphQL.Mvc.GraphQL;

namespace EmployeeManagementGraphQL.Mvc.Models;

public sealed class EmployeesIndexVm
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string SortField { get; init; } = "id";

    public string SortDir { get; init; } = "asc";

    public string? ErrorMessage { get; init; }

    public List<EmployeeRowVm> Items { get; init; } = [];

    public PageInfoVm PageInfo { get; init; } = new()
    {
        Page = 1,
        PageSize = 10,
        TotalCount = 0,
        TotalPages = 0,
        HasNextPage = false,
        HasPreviousPage = false
    };
}
