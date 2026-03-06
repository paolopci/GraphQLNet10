using System.Text.Json.Serialization;

namespace EmployeeManagementGraphQL.Mvc.GraphQL;

public sealed class GraphQlRequest<TVariables>
{
    [JsonPropertyName("query")]
    public required string Query { get; init; }

    [JsonPropertyName("variables")]
    public required TVariables Variables { get; init; }
}

public sealed class GraphQlResponse<TData>
{
    [JsonPropertyName("data")]
    public TData? Data { get; init; }

    [JsonPropertyName("errors")]
    public List<GraphQlError>? Errors { get; init; }
}

public sealed class GraphQlError
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}

public sealed class EmployeesPagedVariables
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("sortBy")]
    public required string SortBy { get; init; }

    [JsonPropertyName("sortDir")]
    public required string SortDir { get; init; }
}

public sealed class EmployeesPagedResponseData
{
    [JsonPropertyName("employeesPaged")]
    public EmployeesPagedPayload? EmployeesPaged { get; init; }
}

public sealed class EmployeesPagedPayload
{
    [JsonPropertyName("items")]
    public List<EmployeeRowVm> Items { get; init; } = [];

    [JsonPropertyName("pageInfo")]
    public PageInfoVm? PageInfo { get; init; }
}

public sealed class EmployeeRowVm
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("lastName")]
    public string LastName { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
}

public sealed class PageInfoVm
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; init; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    [JsonPropertyName("totalPages")]
    public int TotalPages { get; init; }

    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; init; }

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage { get; init; }
}

public sealed class EmployeePagedQueryResult
{
    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public List<EmployeeRowVm> Items { get; init; } = [];

    public PageInfoVm PageInfo { get; init; } = new();
}
