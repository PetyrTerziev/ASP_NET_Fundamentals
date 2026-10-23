using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using GuardPost.Common;
using GuardPost.Data.Models.Enums;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Validation;

using static GuardPost.Common.EntityValidationConstants.Incident;

namespace GuardPost.Web.ViewModels.Incidents;

public class IncidentFormViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Заглавие")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Описание")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [NotInFuture]
    [Display(Name = "Дата и час на събитието")]
    public DateTime? OccurredOn { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [EnumDataType(typeof(IncidentSeverity))]
    [Display(Name = "Сериозност")]
    public IncidentSeverity? Severity { get; set; } = IncidentSeverity.Medium;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [EnumDataType(typeof(IncidentStatus))]
    [Display(Name = "Статус")]
    public IncidentStatus? Status { get; set; } = IncidentStatus.Open;

    [StringLength(ResolutionNotesMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    [Display(Name = "Описание на решението")]
    public string? ResolutionNotes { get; set; }

    [Required(ErrorMessage = ValidationMessages.SelectSite)]
    [Display(Name = "Обект")]
    public int? SiteId { get; set; }

    [Display(Name = "Докладван от")]
    public int? ReportedByGuardId { get; set; }

    [BindNever]
    [ValidateNever]
    public IEnumerable<DropdownItemViewModel> Sites { get; set; } = new List<DropdownItemViewModel>();

    [BindNever]
    [ValidateNever]
    public IEnumerable<DropdownItemViewModel> Guards { get; set; } = new List<DropdownItemViewModel>();

    /// <summary>Cross-field rule that DataAnnotations cannot express.</summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status == IncidentStatus.Resolved && string.IsNullOrWhiteSpace(ResolutionNotes))
        {
            yield return new ValidationResult(
                ValidationMessages.ResolutionNotesRequired,
                new[] { nameof(ResolutionNotes) });
        }
    }
}
