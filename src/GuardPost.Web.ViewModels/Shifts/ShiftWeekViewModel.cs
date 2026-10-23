using GuardPost.Web.ViewModels.Common;

namespace GuardPost.Web.ViewModels.Shifts;

/// <summary>Weekly schedule (Monday – Sunday) with optional filters.</summary>
public class ShiftWeekViewModel
{
    public DateTime WeekStart { get; set; }

    public DateTime WeekEnd => WeekStart.AddDays(6);

    public DateTime Now { get; set; }

    public int? SiteId { get; set; }

    public int? GuardId { get; set; }

    public IEnumerable<DropdownItemViewModel> Sites { get; set; } = new List<DropdownItemViewModel>();

    public IEnumerable<DropdownItemViewModel> Guards { get; set; } = new List<DropdownItemViewModel>();

    public IList<ShiftDayViewModel> Days { get; set; } = new List<ShiftDayViewModel>();

    public int TotalShifts => Days.Sum(d => d.Shifts.Count);

    public double TotalHours => Days.SelectMany(d => d.Shifts).Sum(s => s.DurationHours);
}
