using EmployeeManagementGraphQL.Data.Repositories;
using EmployeeManagementGraphQL.GraphQL.Types;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Queries;

public class EmployeeQuery : ObjectGraphType
{
    public EmployeeQuery(EmployeeRepository employeeRepository)
    {
        Field<ListGraphType<EmployeeGraphType>>(
            "employees",
            resolve: _ => employeeRepository.GetAllEmployees());
    }
}
