using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Domain.Services;

public class WorkScheduleService(IWorkScheduleRepository repository) : IWorkScheduleService
{
    public async Task<WorkSchedule> AddAsync(WorkSchedule workSchedule) =>
        await repository.AddAsync(workSchedule);
}
