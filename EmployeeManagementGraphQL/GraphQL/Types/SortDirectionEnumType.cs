using EmployeeManagementGraphQL.Data.Models.Paging;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class SortDirectionEnumType : EnumerationGraphType<SortDirection>
{
    public SortDirectionEnumType()
    {
        Name = "SortDirection";
        Description = "Allowed sorting direction.";
    }
}
