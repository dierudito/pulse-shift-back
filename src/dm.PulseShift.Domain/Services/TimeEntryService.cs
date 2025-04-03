using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Domain.Services;
public class TimeEntryService(ITimeEntryRepository repository) : ITimeEntryService
{
    public async Task<TimeEntry> AddAsync(TimeEntry timeEntry) => await repository.AddAsync(timeEntry);
}