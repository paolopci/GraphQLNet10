using EmployeeManagementGraphQL.Mvc.GraphQL;

namespace EmployeeManagementGraphQL.Mvc.Models;

public sealed class ReviewsIndexVm
{
    public int? EmployeeId { get; init; }

    public string? ErrorMessage { get; init; }

    public List<ReviewRowVm> Items { get; init; } = [];
}
