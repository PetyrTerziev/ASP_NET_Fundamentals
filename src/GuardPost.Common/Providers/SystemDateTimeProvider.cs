namespace GuardPost.Common.Providers;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;

    public DateTime Today => DateTime.Today;
}
