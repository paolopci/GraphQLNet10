namespace EmployeeManagementGraphQL.Mvc.GraphQL;

public interface IEmployeeGraphQlClient
{
    Task<EmployeePagedQueryResult> GetEmployeesPagedAsync(
        int page,
        int pageSize,
        string sortField,
        string sortDir,
        CancellationToken cancellationToken = default);
}
