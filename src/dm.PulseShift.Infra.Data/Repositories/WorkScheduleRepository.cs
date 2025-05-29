using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.Data.Context;
using dm.PulseShift.Infra.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace dm.PulseShift.Infra.Data.Repositories;

public class WorkScheduleRepository(PulseShiftDbContext db) :
    BaseRepository<WorkSchedule>(db), IWorkScheduleRepository
{
    public async Task<WorkSchedule?> GetByEntryTypeAsync(TimeEntryType entryType) =>
        await _dbSet.FirstOrDefaultAsync(entity => entity.EntryType == entryType);
}
