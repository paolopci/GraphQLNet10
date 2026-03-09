namespace EmployeeManagementGraphQL.Mvc.Models;

public sealed class EmployeeDeleteVm
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string SortField { get; init; } = "id";

    public string SortDir { get; init; } = "asc";
}
