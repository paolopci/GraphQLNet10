using EmployeeManagementGraphQL.Data.Models;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class ReviewGraphType : ObjectGraphType<Review>
{
    public ReviewGraphType()
    {
        Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property for Review object");
        Field(x => x.Rate, type: typeof(IntGraphType)).Description("Rate property for Review object");
        Field(x => x.Comment, type: typeof(StringGraphType)).Description("Comment property for Review object");
        Field(x => x.EmployeeId, type: typeof(IntGraphType)).Description("EmployeeId property for Review object");
        Field<EmployeeGraphType>(
            "employee",
            "Employee associated to the review",
            resolve: context => context.Source?.Employee);
    }
}
