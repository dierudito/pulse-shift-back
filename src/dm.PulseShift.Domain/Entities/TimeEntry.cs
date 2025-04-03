using dm.PulseShift.Domain.Entities.Base;
using dm.PulseShift.Domain.Enums;

namespace dm.PulseShift.Domain.Entities;
public class TimeEntry : Entity
{
    public TimeEntryType EntryType { get; set; } = default!;
    public DateTimeOffset EntryDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
}