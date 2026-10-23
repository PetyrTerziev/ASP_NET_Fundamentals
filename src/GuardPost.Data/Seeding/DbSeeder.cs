using GuardPost.Data.Models;
using GuardPost.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

using static GuardPost.Common.EntityValidationConstants.Shift;

namespace GuardPost.Data.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(GuardPostDbContext context, DateTime today)
    {
        await TranslateLegacyDemoDataAsync(context, today);
       
        if (await context.Sites.IgnoreQueryFilters().AnyAsync()
            || await context.Guards.IgnoreQueryFilters().AnyAsync())
        {
            return;
        }

        var sites = CreateSites(today);
        var guards = CreateGuards();

        await context.Sites.AddRangeAsync(sites);
        await context.Guards.AddRangeAsync(guards);
        await context.SaveChangesAsync();

        var shifts = CreateShifts(sites, guards, today);
        await context.Shifts.AddRangeAsync(shifts);

        var incidents = CreateIncidents(sites, guards, today);
        await context.Incidents.AddRangeAsync(incidents);

        await context.SaveChangesAsync();
    }

    private const string NightShiftNote = "Нощна смяна – обход и запис в дневника на всеки 2 часа.";

    private static readonly string[][] LegacySiteNames =
    [
        ["Logistic Park Trakia - Warehouse B"],
        ["Riverside Business Center", "Бизнес център „Ривърсайд“"],
        ["FreshMarket Supermarket", "Супермаркет „ФрешМаркет“"],
        ["Old Textile Factory"],
    ];

    private static readonly string[] LegacyIncidentTitles =
    [
        "Fence damaged near gate 2",
        "False fire alarm on floor 5",
        "Shoplifting attempt at cash desk 3",
        "Unknown vehicle parked at loading ramp",
        "Access card system offline",
    ];

    private const string LegacyNightShiftNote = "Night shift – patrol log every 2 hours.";
    private const string LegacyLicensePrefix = "SG-";
    private const string LicensePrefix = "ОХ-";

    private static async Task TranslateLegacyDemoDataAsync(GuardPostDbContext context, DateTime today)
    {
        bool changed = false;

        var currentSites = CreateSites(today);
        var sites = await context.Sites.IgnoreQueryFilters().ToListAsync();
        foreach (var site in sites)
        {
            int index = Array.FindIndex(LegacySiteNames, names => names.Contains(site.Name));
            if (index < 0)
            {
                continue;
            }

            var bg = currentSites[index];
            site.Name = bg.Name;
            site.Address = bg.Address;
            site.City = bg.City;
            site.ClientName = bg.ClientName;
            site.Description = bg.Description;
            changed = true;
        }

        foreach (var site in sites)
        {
            var bg = currentSites.FirstOrDefault(s => s.Name == site.Name);
            if (bg != null && site.ClientName != bg.ClientName)
            {
                site.ClientName = bg.ClientName;
                changed = true;
            }
        }

        var currentGuards = CreateGuards();
        var guards = await context.Guards.IgnoreQueryFilters().ToListAsync();
        foreach (var guard in guards.Where(g => g.LicenseNumber.StartsWith(LegacyLicensePrefix)))
        {
            string digits = guard.LicenseNumber.Substring(LegacyLicensePrefix.Length);
            guard.LicenseNumber = LicensePrefix + digits;

            var bg = currentGuards.FirstOrDefault(g => g.LicenseNumber == guard.LicenseNumber);
            if (bg != null)
            {
                guard.FirstName = bg.FirstName;
                guard.LastName = bg.LastName;
            }

            changed = true;
        }

        var currentIncidents = CreateIncidents(currentSites, currentGuards, today);
        var incidents = await context.Incidents.IgnoreQueryFilters().ToListAsync();
        foreach (var incident in incidents)
        {
            int index = Array.IndexOf(LegacyIncidentTitles, incident.Title);
            if (index < 0)
            {
                continue;
            }

            var bg = currentIncidents[index];
            incident.Title = bg.Title;
            incident.Description = bg.Description;
            incident.ResolutionNotes = bg.ResolutionNotes;
            changed = true;
        }

        var shifts = await context.Shifts.IgnoreQueryFilters().Where(s => s.Notes == LegacyNightShiftNote).ToListAsync();
        foreach (var shift in shifts)
        {
            shift.Notes = NightShiftNote;
            changed = true;
        }

        if (changed)
        {
            await context.SaveChangesAsync();
        }
    }

    private static List<Site> CreateSites(DateTime today) =>
    [
        new Site
        {
            Name = "Логистичен парк „Тракия“ – склад Б",
            Address = "ул. „Индустриална“ 12, Северна индустриална зона",
            City = "Стара Загора",
            ClientName = "Тракийски складове ООД",
            ContactPhone = "+359 42 600 111",
            RequiredGuardsPerShift = 1,
            Description = "Денонощна охрана на складово хале от 6 000 м². Проверка на превозните средства на портал 2, обход на всеки 2 часа.",
            IsActive = true,
            CreatedOn = today.AddMonths(-8),
        },
        new Site
        {
            Name = "Бизнес център „Берое“",
            Address = "бул. „Цар Симеон Велики“ 45",
            City = "Стара Загора",
            ClientName = "Берое Имоти АД",
            ContactPhone = "+359 42 611 250",
            RequiredGuardsPerShift = 1,
            Description = "Рецепция и контрол на достъпа в 8-етажна офис сграда. Дневник за ключовете на рецепцията.",
            IsActive = true,
            CreatedOn = today.AddMonths(-5),
        },
        new Site
        {
            Name = "Супермаркет „Свежест“",
            Address = "ул. „Марица“ 3, кв. „Каменица 1“",
            City = "Пловдив",
            ClientName = "Свежест Търговия ЕООД",
            ContactPhone = "+359 32 555 700",
            RequiredGuardsPerShift = 2,
            Description = "Охрана на търговската зала и касите в работно време (08:00 – 20:00).",
            IsActive = true,
            CreatedOn = today.AddMonths(-3),
        },
        new Site
        {
            Name = "Бивша текстилна фабрика",
            Address = "ул. „Заводска“ 1",
            City = "Казанлък",
            ClientName = "Роза Инвест ООД",
            ContactPhone = "+359 431 62 400",
            RequiredGuardsPerShift = 1,
            Description = "Договорът е спрян до започване на ремонта.",
            IsActive = false,
            CreatedOn = today.AddYears(-1),
        },
    ];

    private static List<Guard> CreateGuards() =>
    [
        new Guard { FirstName = "Иван", LastName = "Петров", LicenseNumber = "ОХ-104233", PhoneNumber = "+359 888 120 450", Email = "i.petrov@example.com", HireDate = new DateTime(2019, 3, 1), Rank = GuardRank.Supervisor, IsActive = true },
        new Guard { FirstName = "Георги", LastName = "Димитров", LicenseNumber = "ОХ-118540", PhoneNumber = "+359 887 334 210", Email = "g.dimitrov@example.com", HireDate = new DateTime(2020, 9, 15), Rank = GuardRank.SeniorGuard, IsActive = true },
        new Guard { FirstName = "Мария", LastName = "Колева", LicenseNumber = "ОХ-120017", PhoneNumber = "+359 899 455 002", Email = "m.koleva@example.com", HireDate = new DateTime(2021, 2, 1), Rank = GuardRank.SeniorGuard, IsActive = true },
        new Guard { FirstName = "Стоян", LastName = "Николов", LicenseNumber = "ОХ-131904", PhoneNumber = "+359 878 610 732", HireDate = new DateTime(2022, 6, 20), Rank = GuardRank.Guard, IsActive = true },
        new Guard { FirstName = "Николай", LastName = "Стоев", LicenseNumber = "ОХ-133388", PhoneNumber = "+359 886 900 115", HireDate = new DateTime(2023, 1, 9), Rank = GuardRank.Guard, IsActive = true },
        new Guard { FirstName = "Елена", LastName = "Христова", LicenseNumber = "ОХ-140221", PhoneNumber = "+359 895 210 678", Email = "e.hristova@example.com", HireDate = new DateTime(2023, 10, 2), Rank = GuardRank.Guard, IsActive = true },
        new Guard { FirstName = "Петър", LastName = "Ангелов", LicenseNumber = "ОХ-151760", PhoneNumber = "+359 889 777 340", HireDate = new DateTime(2024, 4, 1), Rank = GuardRank.Guard, IsActive = true },
        new Guard { FirstName = "Димитър", LastName = "Йорданов", LicenseNumber = "ОХ-160045", PhoneNumber = "+359 877 404 990", HireDate = new DateTime(2025, 2, 17), Rank = GuardRank.Trainee, IsActive = true },
        new Guard { FirstName = "Тодор", LastName = "Маринов", LicenseNumber = "ОХ-099871", PhoneNumber = "+359 888 300 222", HireDate = new DateTime(2018, 5, 7), Rank = GuardRank.Guard, IsActive = false },
    ];

    /// <summary>
    /// Builds a schedule from 10 days ago until 14 days ahead. Every slot is given to the active guard
    /// with the fewest hours who has no overlapping shift and enough rest – the same rules the
    /// application enforces when a user creates a shift.
    /// </summary>
    private static List<Shift> CreateShifts(List<Site> sites, List<Guard> guards, DateTime today)
    {
        var activeGuards = guards.Where(g => g.IsActive).ToList();
        var hours = activeGuards.ToDictionary(g => g.Id, _ => 0.0);
        var shifts = new List<Shift>();

        var warehouse = sites[0];
        var businessCenter = sites[1];
        var supermarket = sites[2];

        for (var day = today.AddDays(-10); day <= today.AddDays(14); day = day.AddDays(1))
        {
            var slots = new List<(Site Site, DateTime Start, DateTime End)>
            {
                (warehouse, day.AddHours(8), day.AddHours(20)),
                (businessCenter, day.AddHours(7), day.AddHours(19)),
                (warehouse, day.AddHours(20), day.AddDays(1).AddHours(8)),
                (businessCenter, day.AddHours(19), day.AddDays(1).AddHours(7)),
            };

            if (day.DayOfWeek != DayOfWeek.Sunday)
            {
                for (int i = 0; i < supermarket.RequiredGuardsPerShift; i++)
                {
                    slots.Add((supermarket, day.AddHours(8), day.AddHours(20)));
                }
            }

            foreach (var (site, start, end) in slots.OrderBy(s => s.Start))
            {
                var guard = activeGuards
                    .Where(g => CanTake(shifts, g.Id, start, end))
                    .OrderBy(g => hours[g.Id])
                    .ThenBy(g => g.Id)
                    .FirstOrDefault();

                if (guard is null)
                {
                    continue;
                }

                shifts.Add(new Shift
                {
                    SiteId = site.Id,
                    GuardId = guard.Id,
                    StartTime = start,
                    EndTime = end,
                    Notes = start.Hour >= 19 ? NightShiftNote : null,
                });

                hours[guard.Id] += (end - start).TotalHours;
            }
        }

        return shifts;
    }

    private static bool CanTake(IEnumerable<Shift> existing, int guardId, DateTime start, DateTime end)
        => !existing.Any(s => s.GuardId == guardId
                              && s.StartTime < end.AddHours(MinRestHours)
                              && s.EndTime > start.AddHours(-MinRestHours));

    private static List<Incident> CreateIncidents(List<Site> sites, List<Guard> guards, DateTime today) =>
    [
        new Incident
        {
            Title = "Повредена ограда до портал 2",
            Description = "При нощния обход е установен огънат участък от 2 м на периметровата ограда. Няма следи от проникване в халето.",
            OccurredOn = today.AddDays(-6).AddHours(2).AddMinutes(40),
            Severity = IncidentSeverity.Medium,
            Status = IncidentStatus.Resolved,
            ResolutionNotes = "Възложителят е уведомен; оградата е ремонтирана от поддръжката на следващия ден.",
            SiteId = sites[0].Id,
            ReportedByGuardId = guards[1].Id,
            CreatedOn = today.AddDays(-6).AddHours(3),
        },
        new Incident
        {
            Title = "Фалшива пожарна аларма на 5-ти етаж",
            Description = "Димният датчик в заседателна зала 5.2 задейства алармата. Етажът е евакуиран, не е открит пожар.",
            OccurredOn = today.AddDays(-3).AddHours(14).AddMinutes(5),
            Severity = IncidentSeverity.Low,
            Status = IncidentStatus.Resolved,
            ResolutionNotes = "Датчикът е почистен и тестван от фирмата за пожарна безопасност.",
            SiteId = sites[1].Id,
            ReportedByGuardId = guards[2].Id,
            CreatedOn = today.AddDays(-3).AddHours(14).AddMinutes(30),
        },
        new Incident
        {
            Title = "Опит за кражба на каса 3",
            Description = "Клиент опита да излезе с неплатена електроника. Стоката е върната, извикана е полиция, подписан е протокол.",
            OccurredOn = today.AddDays(-2).AddHours(17).AddMinutes(20),
            Severity = IncidentSeverity.High,
            Status = IncidentStatus.InProgress,
            SiteId = sites[2].Id,
            ReportedByGuardId = guards[3].Id,
            CreatedOn = today.AddDays(-2).AddHours(18),
        },
        new Incident
        {
            Title = "Непознат автомобил на товарната рампа",
            Description = "Сив бус без товарителница е паркиран на рампа 4 повече от час. Шофьорът не е открит.",
            OccurredOn = today.AddDays(-1).AddHours(22).AddMinutes(15),
            Severity = IncidentSeverity.Medium,
            Status = IncidentStatus.Open,
            SiteId = sites[0].Id,
            ReportedByGuardId = guards[4].Id,
            CreatedOn = today.AddDays(-1).AddHours(22).AddMinutes(30),
        },
        new Incident
        {
            Title = "Системата за контрол на достъпа не работи",
            Description = "Турникетите на главния вход спряха да четат картите. Започната е ръчна регистрация на посетителите.",
            OccurredOn = today.AddHours(-3),
            Severity = IncidentSeverity.Critical,
            Status = IncidentStatus.Open,
            SiteId = sites[1].Id,
            CreatedOn = today.AddHours(-3),
        },
    ];
}
