namespace GuardPost.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime date)
    {
        int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.Date.AddDays(-diff);
    }

    public static DateTime StartOfMonth(this DateTime date)
        => new(date.Year, date.Month, 1);

    public static double OverlapHours(DateTime start, DateTime end, DateTime rangeStart, DateTime rangeEnd)
    {
        var from = start > rangeStart ? start : rangeStart;
        var to = end < rangeEnd ? end : rangeEnd;

        return to > from ? (to - from).TotalHours : 0;
    }
}
