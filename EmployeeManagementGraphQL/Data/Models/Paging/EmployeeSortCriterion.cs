namespace EmployeeManagementGraphQL.Data.Models.Paging;

public sealed class EmployeeSortCriterion
{
    public EmployeeSortField Field { get; set; } = EmployeeSortField.Id;

    public SortDirection Direction { get; set; } = SortDirection.Asc;
}
