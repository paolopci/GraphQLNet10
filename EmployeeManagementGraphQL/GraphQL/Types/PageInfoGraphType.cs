using EmployeeManagementGraphQL.Data.Models.Paging;
using GraphQL.Types;

namespace EmployeeManagementGraphQL.GraphQL.Types;

public class PageInfoGraphType : ObjectGraphType<PageInfo>
{
    public PageInfoGraphType()
    {
        Name = "PageInfo";
        Description = "Paging metadata.";

        Field(x => x.Page).Description("Current page number.");
        Field(x => x.PageSize).Description("Current page size.");
        Field(x => x.TotalCount).Description("Total number of records.");
        Field(x => x.TotalPages).Description("Total number of pages.");
        Field(x => x.HasNextPage).Description("Whether there is a next page.");
        Field(x => x.HasPreviousPage).Description("Whether there is a previous page.");
    }
}
