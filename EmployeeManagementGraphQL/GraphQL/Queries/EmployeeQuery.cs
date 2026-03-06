using EmployeeManagementGraphQL.Data.Repositories;
using EmployeeManagementGraphQL.GraphQL.Types;
using GraphQL;
using GraphQL.NewtonsoftJson;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Queries;

public class EmployeeQuery : ObjectGraphType
{
    public EmployeeQuery(EmployeeRepository employeeRepository)
    {
        Field<ListGraphType<EmployeeGraphType>>(
            "employees",
            "Get all employees",
            resolve: _ => employeeRepository.GetAllEmployees());

        Field<EmployeeGraphType>(
           "employeeById",
           "Get employee by id",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id", Description = "Id of employee" }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("id");
                return employeeRepository.GetEmployeeById(id);
            });
    }
}
