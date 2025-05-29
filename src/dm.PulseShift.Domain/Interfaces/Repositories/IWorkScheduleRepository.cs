using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories.Base;

namespace dm.PulseShift.Domain.Interfaces.Repositories;

public interface IWorkScheduleRepository : IBaseRepository<WorkSchedule>
{
    Task<WorkSchedule?> GetByEntryTypeAsync(TimeEntryType entryType);
}
