using dm.PulseShift.Domain.Entities.Base;
using dm.PulseShift.Domain.Enums;

namespace dm.PulseShift.Domain.Entities;
public class TimeEntry : Entity
{
    public TimeEntryType EntryType { get; set; } = default!;
    public DateTime EntryDate { get; set; } = DateTime.Now;
    public DateOnly WorkDate { get; set; } = default!;
    public string? Description { get; set; }

    private readonly List<ActivityPeriod> _startActivityPeriods = [];
    public IReadOnlyCollection<ActivityPeriod> StartActivityPeriods => _startActivityPeriods.AsReadOnly();
    private readonly List<ActivityPeriod> _endActivityPeriods = [];
    public IReadOnlyCollection<ActivityPeriod> EndActivityPeriods => _endActivityPeriods.AsReadOnly();
}