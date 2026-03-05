using EmployeeManagementGraphQL.Data.Models;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class EmployeeGraphType : ObjectGraphType<Employee>
{
    public EmployeeGraphType()
    {
        Field(x => x.Id);
        Field(x => x.FirstName);
        Field(x => x.LastName);
        Field(x => x.Email);
    }
}
