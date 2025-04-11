using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;

namespace dm.PulseShift.Application.AppServices;

public class WorkerAppService(ITimeEntryRepository repository) : IWorkerAppService
{
    public async Task<TimeSpan> GetTodaysDurationAsync()
    {
        var currentDate = DateTimeOffset.UtcNow;
        var entries = await repository.GetByDateAsync(currentDate.Date);
        if (entries is null || !entries.Any())
            return TimeSpan.Zero;

        var clockIn = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockIn)?.EntryDate ?? currentDate;
        var breakStart = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakStart)?.EntryDate ?? currentDate;
        var breakEnd = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakEnd)?.EntryDate ?? currentDate;
        var clockOut = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockOut)?.EntryDate ?? currentDate;

        var preBreakDuration = (breakStart - clockIn).TotalSeconds;
        var postBreakDuration = (clockOut - breakEnd).TotalSeconds;
        var totalDuration = preBreakDuration + postBreakDuration;
        return TimeSpan.FromSeconds(totalDuration);
    }
}
