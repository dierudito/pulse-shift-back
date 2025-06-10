using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories.Base;

namespace dm.PulseShift.Domain.Interfaces.Repositories;

public interface IActivityRepository : IBaseRepository<Activity>
{
    Task<Activity?> GetByIdWithPeriodsAsync(Guid id);
    Task<Activity?> GetByCardCodeWithPeriodsAsync(string cardCode);
    Task<Activity?> GetByCardCodeAsync(string cardCode);
    Task<IEnumerable<Activity>> GetActivitiesIntersectingDateRangeAsync(
        DateTimeOffset rangeStart, DateTimeOffset rangeEnd, Guid? excludeActivityId = null);
    Task<IEnumerable<Activity>> GetCurrentlyActiveActivitiesAsync(DateTimeOffset start, Guid? excludeActivityId = null);
    Task<bool> DoesCardCodeExistAsync(string cardCode);
    Task<(IEnumerable<Activity> Activities, int TotalRecords)> GetActivitiesByDateRangePaginatedAsync(
        DateTimeOffset filterStartDate,
        DateTimeOffset filterEndDate,
        int pageNumber,
        int pageSize);
}