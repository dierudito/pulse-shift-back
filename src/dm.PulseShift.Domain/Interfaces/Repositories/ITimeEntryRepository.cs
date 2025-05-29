using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories.Base;

namespace dm.PulseShift.Domain.Interfaces.Repositories;
public interface ITimeEntryRepository : IBaseRepository<TimeEntry>
{
    Task<IEnumerable<TimeEntry>> GetByDateAsync(DateOnly date);
    Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    Task<TimeEntry?> GetLastAsync();
}