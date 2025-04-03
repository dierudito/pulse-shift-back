using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories.Base;

namespace dm.PulseShift.Domain.Interfaces.Repositories;
public interface ITimeEntryRepository : IBaseRepository<TimeEntry>
{
    Task<IEnumerable<TimeEntry>> GetByDateAsync(DateTimeOffset date);
    Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate);
    Task<TimeEntry?> GetLastAsync();
}