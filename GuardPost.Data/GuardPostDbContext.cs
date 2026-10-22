using GuardPost.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GuardPost.Data;

public class GuardPostDbContext : DbContext
{
    public GuardPostDbContext(DbContextOptions<GuardPostDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Site> Sites { get; set; } = null!;

    public virtual DbSet<Guard> Guards { get; set; } = null!;

    public virtual DbSet<Shift> Shifts { get; set; } = null!;

    public virtual DbSet<Incident> Incidents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GuardPostDbContext).Assembly);
    }
}
