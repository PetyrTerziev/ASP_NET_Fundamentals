using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

using GuardPost.Common;

namespace GuardPost.Web.ViewModels.Validation;

/// <summary>
/// Rejects dates that lie in the future (hire date, time an incident occurred, ...).
/// A small tolerance protects against clock differences between browser and server.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class NotInFutureAttribute : ValidationAttribute, IClientModelValidator
{
    private static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(5);

    public NotInFutureAttribute()
        : base(ValidationMessages.DateInFuture)
    {
    }

    public override string FormatErrorMessage(string name)
        => string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);

    public override bool IsValid(object? value)
        => value is not DateTime date || date <= DateTime.Now.Add(Tolerance);

    public void AddValidation(ClientModelValidationContext context)
    {
        if (!context.Attributes.ContainsKey("data-val"))
        {
            context.Attributes.Add("data-val", "true");
        }

        context.Attributes["data-val-notinfuture"] = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
    }
}
