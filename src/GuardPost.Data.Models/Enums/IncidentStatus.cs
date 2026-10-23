using System.ComponentModel.DataAnnotations;

namespace GuardPost.Data.Models.Enums;

public enum IncidentStatus
{
    [Display(Name = "Отворен")]
    Open = 0,

    [Display(Name = "В процес")]
    InProgress = 1,

    [Display(Name = "Решен")]
    Resolved = 2,
}
