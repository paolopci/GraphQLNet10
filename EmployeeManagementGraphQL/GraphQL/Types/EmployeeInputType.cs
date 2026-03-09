using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class EmployeeInputType : InputObjectGraphType
{
    public EmployeeInputType()
    {
        Name = "EmployeeInput";

        Field<NonNullGraphType<StringGraphType>>("firstName");
        Field<NonNullGraphType<StringGraphType>>("lastName");
        Field<NonNullGraphType<StringGraphType>>("email");
    }
}
