using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Application.AppServices;

public class WorkScheduleAppService(IWorkScheduleRepository repository, ITimeEntryRepository timeEntryRepository, IWorkScheduleService service) : IWorkScheduleAppService
{
    public async Task<bool> AddWorkScheduleAsync(CreateWorkScheduleRequestViewModel requestViewModel)
    {
        if (!await CheckIfInputIsUnique(TimeEntryType.ClockIn)) return false;
        if (!await CheckIfInputIsUnique(TimeEntryType.ClockOut)) return false;
        if (!await CheckIfInputIsUnique(TimeEntryType.BreakStart)) return false;
        if (!await CheckIfInputIsUnique(TimeEntryType.BreakEnd)) return false;

        await AddWorkScheduleAsync(TimeEntryType.ClockIn, requestViewModel.WorkStartTime);
        await AddWorkScheduleAsync(TimeEntryType.ClockOut, requestViewModel.WorkEndTime);
        await AddWorkScheduleAsync(TimeEntryType.BreakStart, requestViewModel.LunchBreakStartTime);
        await AddWorkScheduleAsync(TimeEntryType.BreakEnd, requestViewModel.LunchBreatEndTime);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<long> GetTimeForNextExecAsync()
    {
        var (clockIn, breakStart, breakEnd, clockOut) = await GetWorkingHoursAsync();
        var currentDate = DateTime.Now;
        var lastEntry = await timeEntryRepository.GetLastAsync();

        if (lastEntry is null || lastEntry.EntryDate.Date != currentDate.Date) return GetTimeFromNow(clockIn);
        
        if (lastEntry.EntryType == TimeEntryType.ClockIn)
        {
            var timeUntilBreakStart = breakStart - lastEntry.EntryDate;

            if (timeUntilBreakStart.TotalHours < 2) return (long)TimeSpan.FromHours(2).TotalSeconds;
            return (long)(breakStart - lastEntry.EntryDate).TotalSeconds;
        }

        if (lastEntry.EntryType == TimeEntryType.BreakStart)
        {
            var timeUntilBreakEnd = breakEnd - lastEntry.EntryDate;

            if (timeUntilBreakEnd.TotalHours < 1) return (long)TimeSpan.FromHours(1).TotalSeconds;
            return (long)(breakEnd - lastEntry.EntryDate).TotalSeconds;
        }

        if (lastEntry.EntryType == TimeEntryType.ClockOut)
        {
            var todaysTimeEntry = await timeEntryRepository.GetByDateAsync(DateOnly.FromDateTime(currentDate.Date));
            var halfTime = todaysTimeEntry.FirstOrDefault(entry => entry.EntryType == TimeEntryType.BreakStart)!.EntryDate - 
                           todaysTimeEntry.FirstOrDefault(entry => entry.EntryType == TimeEntryType.ClockIn)!.EntryDate;
            var secondHalfTime = clockOut - 
                               todaysTimeEntry.FirstOrDefault(entry => entry.EntryType == TimeEntryType.BreakEnd)!.EntryDate;
            var totalWorkingTime = halfTime + secondHalfTime;

            if (totalWorkingTime.TotalHours == 8)
                return (long)(clockOut - lastEntry.EntryDate).TotalSeconds;
            if (totalWorkingTime.TotalHours < 8) 
            {
                var fromNowUntilFullTime = 8 - halfTime.TotalHours;
                return (long)TimeSpan.FromHours(fromNowUntilFullTime).TotalSeconds;
            }

            if (totalWorkingTime.TotalHours > 8)
            {
                var fromNowUntilFullTime = totalWorkingTime.TotalHours - 8;
                return (long)TimeSpan.FromHours(fromNowUntilFullTime).TotalSeconds;
            }
        }
        throw new InvalidOperationException("Unable to determine time of next executive");
    }

    private async Task<bool> CheckIfInputIsUnique(TimeEntryType entryType)
    {
        var entity = await repository.GetByEntryTypeAsync(entryType);
        return entity == null;
    }

    private async Task AddWorkScheduleAsync(TimeEntryType entryType, TimeSpan workTime)
    {
        var entity = new WorkSchedule
        {
            EntryType = entryType,
            WorkTime = workTime
        };
        await service.AddAsync(entity);
    }

    private async Task<(DateTime ClockIn, DateTime BreakStart, DateTime BreakEnd, DateTime ClockOut)> GetWorkingHoursAsync()
    {
        var workSchedule = await repository.GetAllAsync();
        
        if (workSchedule == null || !workSchedule.Any()) throw new InvalidOperationException("No work schedule found.");

        var clockInHour = workSchedule.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockIn)!.WorkTime;
        var breakStartHour = workSchedule.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakStart)!.WorkTime;
        var breakEndHour = workSchedule.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakEnd)!.WorkTime;
        var clockOutHour = workSchedule.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockOut)!.WorkTime;
        var currentDate = DateTime.Now;

        var clockIn = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, clockInHour.Hours, clockInHour.Minutes, 0);
        var breakStart = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, breakStartHour.Hours, breakStartHour.Minutes, 0);
        var breakEnd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, breakEndHour.Hours, breakEndHour.Minutes, 0);
        var clockOut = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, clockOutHour.Hours, clockOutHour.Minutes, 0);

        return (clockIn, breakStart, breakEnd, clockOut);
    }

    private static long GetTimeFromNow(DateTime dateTime)
    {
        var currentDate = DateTime.Now;
        var timeSpan = dateTime - currentDate;
        return (long)timeSpan.TotalSeconds;
    }
}
