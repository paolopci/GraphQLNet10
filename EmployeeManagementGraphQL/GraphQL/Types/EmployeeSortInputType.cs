using EmployeeManagementGraphQL.Data.Models.Paging;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class EmployeeSortInputType : InputObjectGraphType<EmployeeSortCriterion>
{
    public EmployeeSortInputType()
    {
        Name = "EmployeeSortInput";
        Description = "Sort criterion for employees list.";

        Field<NonNullGraphType<EmployeeSortFieldEnumType>>("field", "Field used for sorting.");
        Field<SortDirectionEnumType>("direction", "Sorting direction.");
    }
}
