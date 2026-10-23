using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace GuardPost.Web.ViewModels.Validation;

/// <summary>
/// Validates that a DateTime property is strictly later than another DateTime property of the same model.
/// Works on the server (IsValid) and in the browser (IClientModelValidator + wwwroot/js/validation-rules.js).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DateAfterAttribute : ValidationAttribute, IClientModelValidator
{
    public DateAfterAttribute(string otherProperty)
        : base(GuardPost.Common.ValidationMessages.DateAfter)
    {
        OtherProperty = otherProperty;
    }

    public string OtherProperty { get; }

    public override bool RequiresValidationContext => true;

    public override string FormatErrorMessage(string name)
        => string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherProperty);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime current)
        {
            return ValidationResult.Success;
        }

        var otherProperty = validationContext.ObjectType.GetProperty(OtherProperty);
        if (otherProperty is null)
        {
            return new ValidationResult($"Unknown property: {OtherProperty}.");
        }

        if (otherProperty.GetValue(validationContext.ObjectInstance) is DateTime other && current <= other)
        {
            var memberNames = validationContext.MemberName is null
                ? null
                : new[] { validationContext.MemberName };

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
        }

        return ValidationResult.Success;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        Merge(context.Attributes, "data-val", "true");
        Merge(context.Attributes, "data-val-dateafter", FormatErrorMessage(context.ModelMetadata.GetDisplayName()));
        Merge(context.Attributes, "data-val-dateafter-other", OtherProperty);
    }

    private static void Merge(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
        {
            attributes.Add(key, value);
        }
    }
}
