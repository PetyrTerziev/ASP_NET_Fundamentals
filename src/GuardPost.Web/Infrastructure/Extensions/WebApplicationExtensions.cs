using Microsoft.EntityFrameworkCore;

using GuardPost.Common.Providers;
using GuardPost.Data;
using GuardPost.Data.Seeding;

namespace GuardPost.Web.Infrastructure.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Applies pending EF Core migrations and (optionally) seeds demo data.
    /// Controlled by "Database:ApplyMigrationsOnStartup" and "Database:SeedSampleData" in appsettings.
    /// </summary>
    public static async Task PrepareDatabaseAsync(this WebApplication app)
    {
        bool applyMigrations = app.Configuration.GetValue("Database:ApplyMigrationsOnStartup", true);
        bool seedData = app.Configuration.GetValue("Database:SeedSampleData", true);

        if (!applyMigrations && !seedData)
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<GuardPostDbContext>();

        if (applyMigrations)
        {
            await dbContext.Database.MigrateAsync();
        }

        if (seedData)
        {
            var clock = services.GetRequiredService<IDateTimeProvider>();
            await DbSeeder.SeedAsync(dbContext, clock.Today);
        }
    }
}
