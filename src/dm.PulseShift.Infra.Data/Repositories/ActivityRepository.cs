using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.Data.Context;
using dm.PulseShift.Infra.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace dm.PulseShift.Infra.Data.Repositories;

public class ActivityRepository(PulseShiftDbContext db) : BaseRepository<Activity>(db), IActivityRepository
{
    public async Task<bool> DoesCardCodeExistAsync(string cardCode) =>
        await _dbSet.AsNoTracking().AnyAsync(a => a.CardCode == cardCode);

    public async Task<(IEnumerable<Activity> Activities, int TotalRecords)> 
        GetActivitiesByDateRangePaginatedAsync(DateTimeOffset filterStartDate, DateTimeOffset filterEndDate, int pageNumber, int pageSize)
    {

        var query = _dbSet.AsNoTracking()
            .Include(a => a.ActivityPeriods.Where(ap => !ap.IsDeleted))
            .Where(a => !a.IsDeleted &&
                        a.ActivityPeriods.Any(ap => !ap.IsDeleted &&
                                                    ap.StartDate <= filterEndDate &&
                                                    (ap.EndDate == null || ap.EndDate >= filterStartDate)
                        )
            );

        var totalRecords = await query.CountAsync();
        var activities = await query
            .OrderByDescending(a => a.ActivityPeriods.Any(ap => !ap.IsDeleted && ap.EndDate == null))
            .ThenByDescending(a => a.ActivityPeriods
                                    .Where(ap => !ap.IsDeleted && ap.EndDate.HasValue)
                                    .Select(ap => ap.EndDate)
                                    .Max())
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (activities, totalRecords);
    }

    public async Task<IEnumerable<Activity>> GetActivitiesIntersectingDateRangeAsync(
        DateTimeOffset rangeStart,
        DateTimeOffset rangeEnd,
        Guid? excludeActivityId = null)
    {
        var query = _dbSet.AsNoTracking()
            .Include(a => a.ActivityPeriods.Where(ap => !ap.IsDeleted))
            .Where(a => !a.IsDeleted);

        if (excludeActivityId.HasValue)
        {
            query = query.Where(a => a.Id != excludeActivityId.Value);
        }

        query = query.Where(a => a.ActivityPeriods.Any(ap =>
            ap.StartDate <= rangeEnd &&
            (ap.EndDate == null || ap.EndDate >= rangeStart)
        ));

        return await query.ToListAsync();
    }

    public async Task<Activity?> GetByCardCodeAsync(string cardCode) =>
        await _dbSet.FirstOrDefaultAsync(a => a.CardCode == cardCode);

    public async Task<Activity?> GetByCardCodeWithPeriodsAsync(string cardCode) =>
        await _dbSet.Include(a => a.ActivityPeriods).FirstOrDefaultAsync(a => a.CardCode == cardCode);

    public async Task<Activity?> GetByIdWithPeriodsAsync(Guid id) =>
        await _dbSet.Include(a => a.ActivityPeriods).AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Activity>> GetCurrentlyActiveActivitiesAsync(DateTimeOffset start, Guid? excludeActivityId = null)
    {
        var query = _dbSet.AsTracking()
            .Include(a => a.ActivityPeriods.Where(p => !p.IsDeleted))
            .Where(a => !a.IsDeleted && a.ActivityPeriods.Any(ap => !ap.IsDeleted && !ap.EndDate.HasValue && ap.StartDate <= start));

        if (excludeActivityId.HasValue)
        {
            query = query.Where(a => a.Id != excludeActivityId.Value);
        }
        return await query.ToListAsync();
    }
}
