using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class ReviewInputType : InputObjectGraphType
{
    public ReviewInputType()
    {
        Name = "ReviewInput";
        Description = "Input payload used to create or update a review.";

        Field<NonNullGraphType<IntGraphType>>("rate", "Review rate.");
        Field<NonNullGraphType<StringGraphType>>("comment", "Review comment.");
        Field<NonNullGraphType<IntGraphType>>("employeeId", "Target employee id.");
    }
}
