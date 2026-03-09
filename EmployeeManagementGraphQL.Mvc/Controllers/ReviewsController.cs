using EmployeeManagementGraphQL.Mvc.GraphQL;
using EmployeeManagementGraphQL.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementGraphQL.Mvc.Controllers;

public class ReviewsController(IEmployeeGraphQlClient employeeGraphQlClient) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        int? employeeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmployeeId = NormalizeOptionalId(employeeId);

        var result = normalizedEmployeeId.HasValue
            ? await employeeGraphQlClient.GetReviewsByEmployeeIdAsync(normalizedEmployeeId.Value, cancellationToken)
            : await employeeGraphQlClient.GetReviewsAsync(cancellationToken);

        var tempErrorMessage = TempData["ErrorMessage"] as string;
        var effectiveErrorMessage = !string.IsNullOrWhiteSpace(tempErrorMessage)
            ? tempErrorMessage
            : (result.Success ? null : result.ErrorMessage);

        return View(new ReviewsIndexVm
        {
            EmployeeId = normalizedEmployeeId,
            ErrorMessage = effectiveErrorMessage,
            Items = result.Reviews
        });
    }

    [HttpGet]
    public IActionResult Create(int? employeeId = null)
    {
        return View("Upsert", new ReviewUpsertVm
        {
            EmployeeId = NormalizeOptionalId(employeeId) ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        ReviewUpsertVm model,
        CancellationToken cancellationToken = default)
    {
        var normalizedModel = NormalizeUpsertModel(model);
        if (!ModelState.IsValid)
        {
            return View("Upsert", normalizedModel);
        }

        var createResult = await employeeGraphQlClient.AddReviewAsync(
            normalizedModel.Rate,
            normalizedModel.Comment,
            normalizedModel.EmployeeId,
            cancellationToken);

        if (!createResult.Success)
        {
            ModelState.AddModelError(string.Empty, createResult.ErrorMessage ?? "Impossibile creare la review.");
            return View("Upsert", normalizedModel);
        }

        return RedirectToAction(nameof(Index), new
        {
            employeeId = normalizedModel.EmployeeId
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        int id,
        int? employeeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredId(id);
        if (normalizedId <= 0)
        {
            TempData["ErrorMessage"] = "Id review non valido.";
            return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(employeeId) });
        }

        var reviewResult = await employeeGraphQlClient.GetReviewByIdAsync(normalizedId, cancellationToken);
        if (!reviewResult.Success)
        {
            TempData["ErrorMessage"] = reviewResult.ErrorMessage ?? "Errore nel recupero della review.";
            return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(employeeId) });
        }

        if (reviewResult.Review is null)
        {
            TempData["ErrorMessage"] = "Review non trovata.";
            return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(employeeId) });
        }

        return View("Upsert", new ReviewUpsertVm
        {
            Id = reviewResult.Review.Id,
            Rate = reviewResult.Review.Rate,
            Comment = reviewResult.Review.Comment,
            EmployeeId = reviewResult.Review.EmployeeId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        ReviewUpsertVm model,
        CancellationToken cancellationToken = default)
    {
        var normalizedModel = NormalizeUpsertModel(model);
        if (!normalizedModel.Id.HasValue || normalizedModel.Id <= 0)
        {
            ModelState.AddModelError(string.Empty, "Id review non valido.");
            return View("Upsert", normalizedModel);
        }

        if (!ModelState.IsValid)
        {
            return View("Upsert", normalizedModel);
        }

        var updateResult = await employeeGraphQlClient.UpdateReviewAsync(
            normalizedModel.Id.Value,
            normalizedModel.Rate,
            normalizedModel.Comment,
            normalizedModel.EmployeeId,
            cancellationToken);

        if (!updateResult.Success)
        {
            ModelState.AddModelError(string.Empty, updateResult.ErrorMessage ?? "Impossibile aggiornare la review.");
            return View("Upsert", normalizedModel);
        }

        return RedirectToAction(nameof(Index), new
        {
            employeeId = normalizedModel.EmployeeId
        });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        int id,
        int? employeeId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredId(id);
        if (normalizedId <= 0)
        {
            TempData["ErrorMessage"] = "Id review non valido.";
            return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(employeeId) });
        }

        var reviewResult = await employeeGraphQlClient.GetReviewByIdAsync(normalizedId, cancellationToken);
        if (!reviewResult.Success)
        {
            TempData["ErrorMessage"] = reviewResult.ErrorMessage ?? "Errore nel recupero della review.";
            return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(employeeId) });
        }

        if (reviewResult.Review is null)
        {
            TempData["ErrorMessage"] = "Review non trovata.";
            return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(employeeId) });
        }

        return View(new ReviewDeleteVm
        {
            Id = reviewResult.Review.Id,
            Rate = reviewResult.Review.Rate,
            Comment = reviewResult.Review.Comment,
            EmployeeId = reviewResult.Review.EmployeeId
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        ReviewDeleteVm model,
        CancellationToken cancellationToken = default)
    {
        var normalizedId = NormalizeRequiredId(model.Id);
        var normalizedEmployeeId = NormalizeRequiredId(model.EmployeeId);
        if (normalizedId <= 0)
        {
            TempData["ErrorMessage"] = "Id review non valido.";
            return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(normalizedEmployeeId) });
        }

        var deleteResult = await employeeGraphQlClient.DeleteReviewAsync(normalizedId, cancellationToken);
        if (!deleteResult.Success)
        {
            TempData["ErrorMessage"] = deleteResult.ErrorMessage ?? "Impossibile eliminare la review.";
        }

        return RedirectToAction(nameof(Index), new { employeeId = NormalizeOptionalId(normalizedEmployeeId) });
    }

    private static int? NormalizeOptionalId(int? value)
    {
        return value.HasValue && value.Value > 0 ? value : null;
    }

    private static int NormalizeRequiredId(int value)
    {
        return value > 0 ? value : 0;
    }

    private static ReviewUpsertVm NormalizeUpsertModel(ReviewUpsertVm model)
    {
        return new ReviewUpsertVm
        {
            Id = model.Id.HasValue && model.Id.Value > 0 ? model.Id : null,
            Rate = model.Rate > 0 ? model.Rate : 0,
            Comment = model.Comment.Trim(),
            EmployeeId = model.EmployeeId > 0 ? model.EmployeeId : 0
        };
    }
}
