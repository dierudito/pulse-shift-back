using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Domain.Interfaces.Services;

public interface IDayOffService
{
    Task<DayOff> AddAsync(DayOff nonWorkingDay);
    Task<DayOff?> UpdateAsync(DayOff nonWorkingDay);
    Task DeleteAsync(Guid id);
}