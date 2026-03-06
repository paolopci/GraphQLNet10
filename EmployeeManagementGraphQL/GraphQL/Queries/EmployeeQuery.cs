using EmployeeManagementGraphQL.Data.Models.Paging;
using EmployeeManagementGraphQL.Data.Repositories;
using EmployeeManagementGraphQL.GraphQL.Types;
using GraphQL;
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

        Field<EmployeePagedResultGraphType>(
            "employeesPaged",
            "Get paged employees with sorting",
            arguments: new QueryArguments(
                new QueryArgument<IntGraphType> { Name = "page", DefaultValue = 1, Description = "Requested page number." },
                new QueryArgument<IntGraphType> { Name = "pageSize", DefaultValue = 20, Description = "Requested page size." },
                new QueryArgument<EmployeeSortFieldEnumType> { Name = "sortBy", DefaultValue = EmployeeSortField.Id, Description = "Employee sort field." },
                new QueryArgument<SortDirectionEnumType> { Name = "sortDir", DefaultValue = SortDirection.Asc, Description = "Sort direction." }),
            resolve: context =>
            {
                var page = context.GetArgument<int>("page", 1);
                var pageSize = context.GetArgument<int>("pageSize", 20);
                var sortBy = context.GetArgument<EmployeeSortField>("sortBy", EmployeeSortField.Id);
                var sortDir = context.GetArgument<SortDirection>("sortDir", SortDirection.Asc);

                return employeeRepository.GetEmployeesPaged(page, pageSize, sortBy, sortDir);
            });

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
