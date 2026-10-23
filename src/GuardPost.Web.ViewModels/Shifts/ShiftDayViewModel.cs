namespace GuardPost.Web.ViewModels.Shifts;

public class ShiftDayViewModel
{
    public DateTime Date { get; set; }

    public IList<ShiftListItemViewModel> Shifts { get; set; } = new List<ShiftListItemViewModel>();
}
