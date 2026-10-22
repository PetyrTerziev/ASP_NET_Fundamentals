using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GuardPost.Common.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Returns the [Display(Name = ...)] value of an enum member, or the member name if no attribute is set.
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var display = member?.GetCustomAttribute<DisplayAttribute>();

        return display?.GetName() ?? value.ToString();
    }
}
