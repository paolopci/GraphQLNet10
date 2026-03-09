namespace EmployeeManagementGraphQL.Mvc.GraphQL;

public interface IEmployeeGraphQlClient
{
    Task<EmployeePagedQueryResult> GetEmployeesPagedAsync(
        int page,
        int pageSize,
        string sortField,
        string sortDir,
        CancellationToken cancellationToken = default);

    Task<GraphQlOperationResult> DeleteEmployeeAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<EmployeeQueryResult> GetEmployeeByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<EmployeeMutationResult> AddEmployeeAsync(
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default);

    Task<EmployeeMutationResult> UpdateEmployeeAsync(
        int id,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken = default);
}
