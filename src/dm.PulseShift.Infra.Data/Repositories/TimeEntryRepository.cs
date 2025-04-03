using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.Data.Context;
using dm.PulseShift.Infra.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace dm.PulseShift.Infra.Data.Repositories;
public class TimeEntryRepository(PulseShiftDbContext db) : BaseRepository<TimeEntry>(db), ITimeEntryRepository
{
    public async Task<IEnumerable<TimeEntry>> GetByDateAsync(DateTimeOffset date) =>
        await _dbSet.AsNoTracking()
            .Where(te => te.EntryDate.Date == date.Date)
            .ToListAsync();

    public async Task<IEnumerable<TimeEntry>> GetByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate) =>
        await _dbSet.AsNoTracking()
            .Where(te => te.EntryDate.Date >= startDate.Date && te.EntryDate.Date <= endDate.Date)
            .ToListAsync();

    public async Task<TimeEntry?> GetLastAsync() =>
        await _dbSet.AsNoTracking()
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
}
