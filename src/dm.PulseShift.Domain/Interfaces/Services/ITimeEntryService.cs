using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Domain.Interfaces.Services;
public interface ITimeEntryService
{
    Task<TimeEntry> AddAsync(TimeEntry timeEntry);
}