using Microsoft.AspNetCore.Mvc;

using GuardPost.Web.ViewModels.Common;

using static GuardPost.Common.ApplicationConstants;

namespace GuardPost.Web.Controllers;

/// <summary>Shared helpers for all controllers (status messages, business validation errors).</summary>
public abstract class BaseController : Controller
{
    protected void ShowSuccess(string message) => TempData[SuccessMessageKey] = message;

    protected void ShowError(string message) => TempData[ErrorMessageKey] = message;

    protected void AddModelErrors(IEnumerable<ValidationError> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.PropertyName, error.Message);
        }
    }
}
