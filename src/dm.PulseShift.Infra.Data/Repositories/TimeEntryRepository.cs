using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.Data.Context;
using dm.PulseShift.Infra.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace dm.PulseShift.Infra.Data.Repositories;
public class TimeEntryRepository(PulseShiftDbContext db) : BaseRepository<TimeEntry>(db), ITimeEntryRepository
{
    public async Task<IEnumerable<TimeEntry>> GetByDateAsync(DateOnly date) =>
        await _dbSet.AsNoTracking()
            .Where(te => te.WorkDate == date)
            .ToListAsync();

    public async Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate) =>
        await _dbSet.AsNoTracking()
            .Where(te => !te.IsDeleted && te.WorkDate >= startDate && te.WorkDate <= endDate)
            .OrderBy(te => te.WorkDate)
            .ThenBy(te => te.EntryDate)
            .ToListAsync();

    public async Task<TimeEntry?> GetLastAsync() =>
        await _dbSet.AsNoTracking()
            .Where(te => !te.IsDeleted)
            .OrderByDescending(te => te.EntryDate)
            .FirstOrDefaultAsync();

    public override async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            entity.MarkAsDeleted();
            _dbSet.Update(entity);
        }
    }

    public async Task<IEnumerable<TimeEntry>> GetEntriesByDateRangeOrderedAsync(DateTime startDateTime, DateTime endDateTime)
    {
        var query = _dbSet.AsNoTracking()
            .Where(te => !te.IsDeleted && te.EntryDate >= startDateTime && te.EntryDate <= endDateTime)
            .OrderBy(te => te.EntryDate);

        var sql = query.ToQueryString();
        return await query.ToListAsync();
    }
}
