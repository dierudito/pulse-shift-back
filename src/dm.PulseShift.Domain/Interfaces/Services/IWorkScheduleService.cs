using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Domain.Interfaces.Services;

public interface IWorkScheduleService
{
    Task<WorkSchedule> AddAsync(WorkSchedule workSchedule);
}