using EmployeeManagementGraphQL.Data.Models;
using GraphQL;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class EmployeeGraphType : ObjectGraphType<Employee>
{
    public EmployeeGraphType()
    {
        Field(x => x.Id, type: typeof(IdGraphType)).Description("Id property for Employee object");
        Field(x => x.FirstName, type: typeof(StringGraphType)).Description("FirstName property for Employee object");
        Field(x => x.LastName, type: typeof(StringGraphType)).Description("LastName property for Employee object");
        Field(x => x.Email, type: typeof(StringGraphType)).Description("Email property for Employee object");
        Field<NonNullGraphType<IntGraphType>>(
            "reviewsCount",
            "Number of reviews associated with the employee",
            resolve: context => context.Source?.ReviewsCount ?? 0);
        Field<ListGraphType<ReviewGraphType>>(
            "reviews",
            "Reviews associated with the employee",
            resolve: context => context.Source?.Reviews ?? []);
    }
}
