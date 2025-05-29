using dm.PulseShift.Domain.Entities.Base;
using dm.PulseShift.Domain.Enums;

namespace dm.PulseShift.Domain.Entities;

public class WorkSchedule : Entity
{
    public TimeSpan WorkTime { get; set; }
    public TimeEntryType EntryType { get; set; }
}