using System.ComponentModel.DataAnnotations;

namespace GuardPost.Data.Models.Enums;

public enum GuardRank
{
    [Display(Name = "Стажант")]
    Trainee = 0,

    [Display(Name = "Охранител")]
    Guard = 1,

    [Display(Name = "Старши охранител")]
    SeniorGuard = 2,

    [Display(Name = "Началник смяна")]
    Supervisor = 3,
}
