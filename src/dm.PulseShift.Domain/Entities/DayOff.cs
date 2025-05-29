using dm.PulseShift.Domain.Entities.Base;

namespace dm.PulseShift.Domain.Entities;

public class DayOff : Entity
{
    public DateOnly Date { get; set; }
    public string? Description { get; set; }
}