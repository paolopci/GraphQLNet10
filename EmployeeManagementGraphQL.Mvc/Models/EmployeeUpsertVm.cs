using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementGraphQL.Mvc.Models;

public sealed class EmployeeUpsertVm
{
    public int? Id { get; init; }

    [Display(Name = "First Name")]
    [Required(ErrorMessage = "Il First Name è obbligatorio.")]
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Display(Name = "Last Name")]
    [Required(ErrorMessage = "Il Last Name è obbligatorio.")]
    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Display(Name = "Email")]
    [Required(ErrorMessage = "L'Email è obbligatoria.")]
    [EmailAddress(ErrorMessage = "Formato email non valido.")]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string SortField { get; init; } = "id";

    public string SortDir { get; init; } = "asc";

    public bool IsEdit => Id.HasValue;
}
