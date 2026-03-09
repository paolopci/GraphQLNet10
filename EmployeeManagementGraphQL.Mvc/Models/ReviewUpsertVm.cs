using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementGraphQL.Mvc.Models;

public sealed class ReviewUpsertVm
{
    public int? Id { get; init; }

    [Display(Name = "Rate")]
    [Range(1, 5, ErrorMessage = "Il rate deve essere tra 1 e 5.")]
    public int Rate { get; init; }

    [Display(Name = "Comment")]
    [Required(ErrorMessage = "Il commento è obbligatorio.")]
    [StringLength(500, ErrorMessage = "Il commento non può superare 500 caratteri.")]
    public string Comment { get; init; } = string.Empty;

    [Display(Name = "Employee Id")]
    [Range(1, int.MaxValue, ErrorMessage = "EmployeeId non valido.")]
    public int EmployeeId { get; init; }

    public bool IsEdit => Id.HasValue;
}
