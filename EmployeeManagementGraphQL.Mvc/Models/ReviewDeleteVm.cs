namespace EmployeeManagementGraphQL.Mvc.Models;

public sealed class ReviewDeleteVm
{
    public int Id { get; init; }

    public int Rate { get; init; }

    public string Comment { get; init; } = string.Empty;

    public int EmployeeId { get; init; }
}
