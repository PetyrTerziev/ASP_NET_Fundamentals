namespace GuardPost.Common;

public static class ValidationMessages
{
    public const string Required = "Полето „{0}“ е задължително.";
    public const string StringLength = "Полето „{0}“ трябва да бъде между {2} и {1} символа.";
    public const string MaxLength = "Полето „{0}“ не може да бъде по-дълго от {1} символа.";
    public const string Range = "Полето „{0}“ трябва да бъде между {1} и {2}.";
    public const string InvalidPhone = "Въведете валиден телефонен номер (цифри и интервали, по желание + в началото).";
    public const string InvalidEmail = "Въведете валиден имейл адрес.";
    public const string InvalidLicenseNumber = "Номерът на удостоверението трябва да е във формат ОХ-123456 (2 главни букви на кирилица, тире, 6 цифри).";
    public const string LicenseNumberTaken = "Вече има охранител с удостоверение № {0}.";
    public const string DateAfter = "„{0}“ трябва да бъде след „{1}“.";
    public const string EndMustBeAfterStart = "Краят на смяната трябва да бъде след началото ѝ.";
    public const string DateInFuture = "„{0}“ не може да бъде в бъдещето.";
    public const string SelectSite = "Моля, изберете обект.";
    public const string SelectGuard = "Моля, изберете охранител.";
    public const string ResolutionNotesRequired = "Описанието на решението е задължително, когато инцидентът е със статус „Решен“.";

    public const string SiteNotAvailable = "Избраният обект не съществува или е неактивен.";
    public const string GuardNotAvailable = "Избраният охранител не съществува или е неактивен.";
    public const string ShiftDurationOutOfRange = "Смяната трябва да продължава между {0} и {1} часа.";
    public const string ShiftOverlaps = "{0} вече има смяна на обект {1}, която се застъпва с този период ({2}).";
    public const string ShiftRestViolation = "{0} трябва да има поне {1} часа почивка между смените – конфликт със смяна на обект {2} ({3}).";

    // Default model-binding messages (used when a value cannot be parsed at all).
    public const string ValueIsInvalid = "Стойността „{0}“ е невалидна за полето „{1}“.";
    public const string ValueMustBeANumber = "Полето „{0}“ трябва да бъде число.";
    public const string ValueRequired = "Полето е задължително.";
}
