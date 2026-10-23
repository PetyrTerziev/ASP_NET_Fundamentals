namespace GuardPost.Common.Providers;

public interface IDateTimeProvider
{
    DateTime Now { get; }

    DateTime Today { get; }
}
