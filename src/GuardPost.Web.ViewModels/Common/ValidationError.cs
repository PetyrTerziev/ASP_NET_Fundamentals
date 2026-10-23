namespace GuardPost.Web.ViewModels.Common;

/// <summary>
/// A business-rule violation found by a service. PropertyName is empty for model-level errors.
/// </summary>
public record ValidationError(string PropertyName, string Message);
