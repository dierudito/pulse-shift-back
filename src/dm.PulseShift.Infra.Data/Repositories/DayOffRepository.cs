using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.Data.Context;
using dm.PulseShift.Infra.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace dm.PulseShift.Infra.Data.Repositories;

public class DayOffRepository(PulseShiftDbContext db) :
    BaseRepository<DayOff>(db), IDayOffRepository
{
    public async Task<DayOff?> GetByDateAsync(DateOnly date) =>
        await _dbSet.FirstOrDefaultAsync(entity => entity.Date == date);

    public async Task<IEnumerable<DayOff>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate) =>
        await _dbSet.Where(entity => entity.Date >= startDate && entity.Date < endDate).ToListAsync();
}
