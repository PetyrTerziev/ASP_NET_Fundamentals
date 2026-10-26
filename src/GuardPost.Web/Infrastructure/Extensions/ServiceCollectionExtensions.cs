using Microsoft.EntityFrameworkCore;

using GuardPost.Common.Providers;
using GuardPost.Data;
using GuardPost.Services.Core;
using GuardPost.Services.Core.Contracts;

namespace GuardPost.Web.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<GuardPostDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }

    /// <summary>Registers the business services – controllers depend only on the interfaces.</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        services.AddScoped<ISiteService, SiteService>();
        services.AddScoped<IGuardService, GuardService>();
        services.AddScoped<IShiftService, ShiftService>();
        services.AddScoped<IIncidentService, IncidentService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
