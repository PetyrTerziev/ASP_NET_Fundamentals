using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using GuardPost.Common;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Validation;

using static GuardPost.Common.EntityValidationConstants.Shift;

namespace GuardPost.Web.ViewModels.Shifts;

public class ShiftFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.SelectSite)]
    [Display(Name = "Обект")]
    public int? SiteId { get; set; }

    [Required(ErrorMessage = ValidationMessages.SelectGuard)]
    [Display(Name = "Охранител")]
    public int? GuardId { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [Display(Name = "Начало")]
    public DateTime? StartTime { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [DateAfter(nameof(StartTime), ErrorMessage = ValidationMessages.EndMustBeAfterStart)]
    [Display(Name = "Край")]
    public DateTime? EndTime { get; set; }

    [StringLength(NotesMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    [Display(Name = "Бележки")]
    public string? Notes { get; set; }

    [BindNever]
    [ValidateNever]
    public IEnumerable<DropdownItemViewModel> Sites { get; set; } = new List<DropdownItemViewModel>();

    [BindNever]
    [ValidateNever]
    public IEnumerable<DropdownItemViewModel> Guards { get; set; } = new List<DropdownItemViewModel>();
}
