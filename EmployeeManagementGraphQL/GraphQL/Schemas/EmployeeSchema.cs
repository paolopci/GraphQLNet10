using EmployeeManagementGraphQL.GraphQL.Mutations;
using EmployeeManagementGraphQL.GraphQL.Queries;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Schemas;

public class EmployeeSchema : Schema
{
    public EmployeeSchema(
        IServiceProvider serviceProvider,
        EmployeeQuery query,
        EmployeeMutation mutation)
        : base(serviceProvider)
    {
        Query = query;
        Mutation = mutation;
    }
}
