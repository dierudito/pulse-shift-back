using dm.PulseShift.Domain.Entities.Base;
using dm.PulseShift.Domain.Enums;

namespace dm.PulseShift.Domain.Entities;
public class TimeEntry : Entity
{
    public TimeEntryType EntryType { get; set; } = default!;
    public DateTimeOffset EntryDate { get; set; } = DateTime.UtcNow;
    public DateOnly WorkDate { get; set; } = default!;
    public string? Description { get; set; }

    private readonly List<ActivityPeriod> _startActivityPeriods = new();
    public IReadOnlyCollection<ActivityPeriod> StartActivityPeriods => _startActivityPeriods.AsReadOnly();
    private readonly List<ActivityPeriod> _endActivityPeriods = new();
    public IReadOnlyCollection<ActivityPeriod> EndActivityPeriods => _endActivityPeriods.AsReadOnly();
}