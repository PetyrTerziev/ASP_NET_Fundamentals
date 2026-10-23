namespace GuardPost.Data.Models;

public class Site
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public int RequiredGuardsPerShift { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Shift> Shifts { get; set; } = new HashSet<Shift>();

    public virtual ICollection<Incident> Incidents { get; set; } = new HashSet<Incident>();
}
