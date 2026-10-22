namespace GuardPost.Common;

/// <summary>
/// Single source of truth for all length / range / format restrictions.
/// Used both by the EF Core configurations (database schema) and by the
/// view models (server-side + client-side validation).
/// </summary>
public static class EntityValidationConstants
{
    public static class Shared
    {
        public const int PhoneMaxLength = 20;
        public const string PhoneRegex = @"^\+?[0-9][0-9 ]{5,18}$";
    }

    public static class Site
    {
        public const int NameMinLength = 3;
        public const int NameMaxLength = 100;

        public const int AddressMinLength = 5;
        public const int AddressMaxLength = 200;

        public const int CityMinLength = 2;
        public const int CityMaxLength = 50;

        public const int ClientNameMinLength = 2;
        public const int ClientNameMaxLength = 100;

        public const int DescriptionMaxLength = 1000;

        public const int RequiredGuardsMin = 1;
        public const int RequiredGuardsMax = 20;
    }

    public static class Guard
    {
        public const int FirstNameMinLength = 2;
        public const int FirstNameMaxLength = 50;

        public const int LastNameMinLength = 2;
        public const int LastNameMaxLength = 50;

        /// <summary>Format: two capital Cyrillic letters, dash, six digits – e.g. ОХ-104233.</summary>
        public const int LicenseNumberMaxLength = 9;
        public const string LicenseNumberRegex = @"^[А-Я]{2}-\d{6}$";

        public const int EmailMaxLength = 100;
    }

    public static class Shift
    {
        public const int NotesMaxLength = 500;

        public const int MinDurationHours = 1;
        public const int MaxDurationHours = 24;

        /// <summary>Minimum uninterrupted rest between two shifts of the same guard.</summary>
        public const int MinRestHours = 12;
    }

    public static class Incident
    {
        public const int TitleMinLength = 5;
        public const int TitleMaxLength = 100;

        public const int DescriptionMinLength = 10;
        public const int DescriptionMaxLength = 2000;

        public const int ResolutionNotesMaxLength = 1000;
    }
}
