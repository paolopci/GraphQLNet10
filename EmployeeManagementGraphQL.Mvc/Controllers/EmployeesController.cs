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

        var tempErrorMessage = TempData["ErrorMessage"] as string;
        var effectiveErrorMessage = !string.IsNullOrWhiteSpace(tempErrorMessage)
            ? tempErrorMessage
            : (result.Success ? null : result.ErrorMessage);

        var viewModel = new EmployeesIndexVm
        {
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            SortField = normalizedSortField,
            SortDir = normalizedSortDir,
            ErrorMessage = effectiveErrorMessage,
            Items = result.Items,
            PageInfo = result.PageInfo
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create(
        int page = 1,
        int pageSize = 10,
        string sortField = "id",
        string sortDir = "asc")
    {
        return View("Upsert", new EmployeeUpsertVm
        {
            Page = page < 1 ? 1 : page,
            PageSize = pageSize <= 0 ? 10 : pageSize,
            SortField = NormalizeSortField(sortField),
            SortDir = NormalizeSortDir(sortDir)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EmployeeUpsertVm model,
        CancellationToken cancellationToken = default)
    {
        var normalizedModel = new EmployeeUpsertVm
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Page = model.Page < 1 ? 1 : model.Page,
            PageSize = model.PageSize <= 0 ? 10 : model.PageSize,
            SortField = NormalizeSortField(model.SortField),
            SortDir = NormalizeSortDir(model.SortDir)
        };

        if (!ModelState.IsValid)
        {
            return View("Upsert", normalizedModel);
        }

        var createResult = await employeeGraphQlClient.AddEmployeeAsync(
            normalizedModel.FirstName,
            normalizedModel.LastName,
            normalizedModel.Email,
            cancellationToken);

        if (!createResult.Success)
        {
            ModelState.AddModelError(string.Empty, createResult.ErrorMessage ?? "Impossibile creare il dipendente.");
            return View("Upsert", normalizedModel);
        }

        return RedirectToAction(nameof(Index), new
        {
            page = normalizedModel.Page,
            pageSize = normalizedModel.PageSize,
            sortField = normalizedModel.SortField,
            sortDir = normalizedModel.SortDir
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
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

        var employeeResult = await employeeGraphQlClient.GetEmployeeByIdAsync(id, cancellationToken);
        if (!employeeResult.Success)
        {
            TempData["ErrorMessage"] = employeeResult.ErrorMessage ?? "Errore nel recupero del dipendente.";
            return RedirectToAction(nameof(Index), new
            {
                page = normalizedPage,
                pageSize = normalizedPageSize,
                sortField = normalizedSortField,
                sortDir = normalizedSortDir
            });
        }

        if (employeeResult.Employee is null)
        {
            TempData["ErrorMessage"] = "Dipendente non trovato.";
            return RedirectToAction(nameof(Index), new
            {
                page = normalizedPage,
                pageSize = normalizedPageSize,
                sortField = normalizedSortField,
                sortDir = normalizedSortDir
            });
        }

        return View("Upsert", new EmployeeUpsertVm
        {
            Id = employeeResult.Employee.Id,
            FirstName = employeeResult.Employee.FirstName,
            LastName = employeeResult.Employee.LastName,
            Email = employeeResult.Employee.Email,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            SortField = normalizedSortField,
            SortDir = normalizedSortDir
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        EmployeeUpsertVm model,
        CancellationToken cancellationToken = default)
    {
        var normalizedModel = new EmployeeUpsertVm
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Page = model.Page < 1 ? 1 : model.Page,
            PageSize = model.PageSize <= 0 ? 10 : model.PageSize,
            SortField = NormalizeSortField(model.SortField),
            SortDir = NormalizeSortDir(model.SortDir)
        };

        if (!normalizedModel.Id.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Id dipendente non valido.");
            return View("Upsert", normalizedModel);
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", normalizedModel);
        }

        var updateResult = await employeeGraphQlClient.UpdateEmployeeAsync(
            normalizedModel.Id.Value,
            normalizedModel.FirstName,
            normalizedModel.LastName,
            normalizedModel.Email,
            cancellationToken);

        if (!updateResult.Success)
        {
            ModelState.AddModelError(string.Empty, updateResult.ErrorMessage ?? "Impossibile aggiornare il dipendente.");
            return View("Upsert", normalizedModel);
        }

        return RedirectToAction(nameof(Index), new
        {
            page = normalizedModel.Page,
            pageSize = normalizedModel.PageSize,
            sortField = normalizedModel.SortField,
            sortDir = normalizedModel.SortDir
        });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        int id,
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

        var employeeResult = await employeeGraphQlClient.GetEmployeeByIdAsync(id, cancellationToken);
        if (!employeeResult.Success)
        {
            TempData["ErrorMessage"] = employeeResult.ErrorMessage ?? "Errore nel recupero del dipendente.";
            return RedirectToAction(nameof(Index), new
            {
                page = normalizedPage,
                pageSize = normalizedPageSize,
                sortField = normalizedSortField,
                sortDir = normalizedSortDir
            });
        }

        if (employeeResult.Employee is null)
        {
            TempData["ErrorMessage"] = "Dipendente non trovato.";
            return RedirectToAction(nameof(Index), new
            {
                page = normalizedPage,
                pageSize = normalizedPageSize,
                sortField = normalizedSortField,
                sortDir = normalizedSortDir
            });
        }

        return View(new EmployeeDeleteVm
        {
            Id = employeeResult.Employee.Id,
            FirstName = employeeResult.Employee.FirstName,
            LastName = employeeResult.Employee.LastName,
            Email = employeeResult.Employee.Email,
            Page = normalizedPage,
            PageSize = normalizedPageSize,
            SortField = normalizedSortField,
            SortDir = normalizedSortDir
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        EmployeeDeleteVm model,
        CancellationToken cancellationToken = default)
    {
        var normalizedPage = model.Page < 1 ? 1 : model.Page;
        var normalizedPageSize = model.PageSize <= 0 ? 10 : model.PageSize;
        var normalizedSortField = NormalizeSortField(model.SortField);
        var normalizedSortDir = NormalizeSortDir(model.SortDir);

        var deleteResult = await employeeGraphQlClient.DeleteEmployeeAsync(model.Id, cancellationToken);
        if (!deleteResult.Success)
        {
            TempData["ErrorMessage"] = deleteResult.ErrorMessage ?? "Impossibile eliminare il dipendente.";
        }

        return RedirectToAction(nameof(Index), new
        {
            page = normalizedPage,
            pageSize = normalizedPageSize,
            sortField = normalizedSortField,
            sortDir = normalizedSortDir
        });
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
