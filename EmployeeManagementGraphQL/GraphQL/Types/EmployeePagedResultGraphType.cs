using EmployeeManagementGraphQL.Data.Models;
using EmployeeManagementGraphQL.Data.Models.Paging;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class EmployeePagedResultGraphType : ObjectGraphType<PagedResult<Employee>>
{
    public EmployeePagedResultGraphType()
    {
        Name = "EmployeePagedResult";
        Description = "Paged employee response.";

        Field<NonNullGraphType<ListGraphType<NonNullGraphType<EmployeeGraphType>>>>(
            "items",
            "Current page items.",
            resolve: context => context.Source!.Items);

        Field<NonNullGraphType<PageInfoGraphType>>(
            "pageInfo",
            "Paging information.",
            resolve: context => context.Source!.PageInfo);
    }
}
