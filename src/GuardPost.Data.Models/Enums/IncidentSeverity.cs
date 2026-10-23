using System.ComponentModel.DataAnnotations;

namespace GuardPost.Data.Models.Enums;

public enum IncidentSeverity
{
    [Display(Name = "Ниска")]
    Low = 0,

    [Display(Name = "Средна")]
    Medium = 1,

    [Display(Name = "Висока")]
    High = 2,

    [Display(Name = "Критична")]
    Critical = 3,
}
