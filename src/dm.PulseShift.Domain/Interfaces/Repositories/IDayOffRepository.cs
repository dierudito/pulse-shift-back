using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories.Base;

namespace dm.PulseShift.Domain.Interfaces.Repositories;

public interface IDayOffRepository : IBaseRepository<DayOff>
{
    Task<DayOff?> GetByDateAsync(DateOnly date);
    Task<IEnumerable<DayOff>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
}