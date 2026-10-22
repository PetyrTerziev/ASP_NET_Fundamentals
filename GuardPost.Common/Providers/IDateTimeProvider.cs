namespace GuardPost.Common.Providers;

/// <summary>
/// Abstraction over the system clock so that services stay testable (Dependency Inversion).
/// </summary>
public interface IDateTimeProvider
{
    DateTime Now { get; }

    DateTime Today { get; }
}
