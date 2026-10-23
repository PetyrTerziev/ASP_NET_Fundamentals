using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using GuardPost.Common;
using GuardPost.Data.Models.Enums;
using GuardPost.Web.ViewModels.Validation;

using static GuardPost.Common.EntityValidationConstants.Guard;
using static GuardPost.Common.EntityValidationConstants.Shared;

namespace GuardPost.Web.ViewModels.Guards;

public class GuardFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Име")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Фамилия")]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// [Remote] asks the server (GuardsController.CheckLicenseNumber) while the user types,
    /// the same check is repeated on POST.
    /// </summary>
    [Required(ErrorMessage = ValidationMessages.Required)]
    [RegularExpression(LicenseNumberRegex, ErrorMessage = ValidationMessages.InvalidLicenseNumber)]
    [Remote(action: "CheckLicenseNumber", controller: "Guards", AdditionalFields = nameof(Id))]
    [Display(Name = "Удостоверение №")]
    public string LicenseNumber { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(PhoneMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    [RegularExpression(PhoneRegex, ErrorMessage = ValidationMessages.InvalidPhone)]
    [Display(Name = "Телефон")]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(EmailMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    [EmailAddress(ErrorMessage = ValidationMessages.InvalidEmail)]
    [Display(Name = "Имейл")]
    public string? Email { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [DataType(DataType.Date)]
    [NotInFuture]
    [Display(Name = "Дата на назначаване")]
    public DateTime? HireDate { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [EnumDataType(typeof(GuardRank))]
    [Display(Name = "Ранг")]
    public GuardRank? Rank { get; set; } = GuardRank.Guard;

    [Display(Name = "Активен (може да поема смени)")]
    public bool IsActive { get; set; } = true;
}
