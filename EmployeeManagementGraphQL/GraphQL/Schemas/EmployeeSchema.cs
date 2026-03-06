using EmployeeManagementGraphQL.GraphQL.Queries;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Schemas;

public class EmployeeSchema : Schema
{
    public EmployeeSchema(IServiceProvider serviceProvider, EmployeeQuery query)
        : base(serviceProvider)
    {
        Query = query;
    }
}
