using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Domain.Services;

public class DayOffService(IDayOffRepository repository) : IDayOffService
{
    public async Task<DayOff> AddAsync(DayOff nonWorkingDay) =>
        await repository.AddAsync(nonWorkingDay);
    public async Task<DayOff?> UpdateAsync(DayOff nonWorkingDay) =>
        await repository.UpdateAsync(nonWorkingDay, nonWorkingDay.Id);
    public async Task DeleteAsync(Guid id) =>
        await repository.DeleteAsync(id);
}