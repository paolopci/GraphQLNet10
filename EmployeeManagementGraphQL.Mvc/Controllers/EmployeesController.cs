using EmployeeManagementGraphQL.Mvc.GraphQL;
using EmployeeManagementGraphQL.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementGraphQL.Mvc.Controllers;

public class EmployeesController(IEmployeeGraphQlClient employeeGraphQlClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        int page = 1,
        int pageSize = 10,
        string sortField = "id",
        string sortDir = "asc",
        CancellationToken cancellationToken = default)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedPageSize = pageSize <= 0 ? 10 : pageSize;
        var normalizedSortField = NormalizeSortField(sortField);
        var normalizedSortDir = NormalizeSortDir(sortDir);

        var result = await employeeGraphQlClient.GetEmployeesPagedAsync(
            normalizedPage,
            normalizedPageSize,
            normalizedSortField,
            normalizedSortDir,
            cancellationToken);

        var viewModel = new EmployeesIndexVm
        {
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            SortField = normalizedSortField,
            SortDir = normalizedSortDir,
            ErrorMessage = result.Success ? null : result.ErrorMessage,
            Items = result.Items,
            PageInfo = result.PageInfo
        };

        return View(viewModel);
    }

    private static string NormalizeSortField(string? sortField)
    {
        return sortField?.Trim().ToLowerInvariant() switch
        {
            "firstname" => "firstname",
            "lastname" => "lastname",
            "email" => "email",
            _ => "id"
        };
    }

    private static string NormalizeSortDir(string? sortDir)
    {
        return sortDir?.Trim().ToLowerInvariant() switch
        {
            "desc" => "desc",
            _ => "asc"
        };
    }
}
