using System.ComponentModel.DataAnnotations;

using GuardPost.Common;

using static GuardPost.Common.EntityValidationConstants.Shared;
using static GuardPost.Common.EntityValidationConstants.Site;

namespace GuardPost.Web.ViewModels.Sites;

/// <summary>Used by both Create and Edit.</summary>
public class SiteFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Име на обекта")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(AddressMaxLength, MinimumLength = AddressMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Адрес")]
    public string Address { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(CityMaxLength, MinimumLength = CityMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Град")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(ClientNameMaxLength, MinimumLength = ClientNameMinLength, ErrorMessage = ValidationMessages.StringLength)]
    [Display(Name = "Възложител")]
    public string ClientName { get; set; } = null!;

    [Required(ErrorMessage = ValidationMessages.Required)]
    [StringLength(PhoneMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    [RegularExpression(PhoneRegex, ErrorMessage = ValidationMessages.InvalidPhone)]
    [Display(Name = "Телефон за връзка")]
    public string ContactPhone { get; set; } = null!;

    [Range(RequiredGuardsMin, RequiredGuardsMax, ErrorMessage = ValidationMessages.Range)]
    [Display(Name = "Необходими охранители на смяна")]
    public int RequiredGuardsPerShift { get; set; } = 1;

    [StringLength(DescriptionMaxLength, ErrorMessage = ValidationMessages.MaxLength)]
    [Display(Name = "Описание / постови инструкции")]
    public string? Description { get; set; }

    [Display(Name = "Активен договор")]
    public bool IsActive { get; set; } = true;
}
