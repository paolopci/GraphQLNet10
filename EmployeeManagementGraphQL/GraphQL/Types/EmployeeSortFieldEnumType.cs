using EmployeeManagementGraphQL.Data.Models.Paging;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class EmployeeSortFieldEnumType : EnumerationGraphType<EmployeeSortField>
{
    public EmployeeSortFieldEnumType()
    {
        Name = "EmployeeSortField";
        Description = "Allowed fields for employee sorting.";
    }
}
