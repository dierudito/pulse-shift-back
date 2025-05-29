using AutoMapper;
using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;
using System.Net;

namespace dm.PulseShift.Application.AppServices;
public class TimeEntryAppService(
    ITimeEntryRepository repository, 
    ITimeEntryService service,
    IDayOffRepository nonWorkingDayRepository,
    IMapper mapper) : ITimeEntryAppService
{
    public async Task<Response<TimeEntryResponseViewModel>> CreateAsync()
    {
        var currentDate = DateTimeOffset.UtcNow;
        var lastEntry = await repository.GetLastAsync();
        var entryType = TimeEntryType.ClockIn;

        if (lastEntry is not null && lastEntry.EntryDate.Date == currentDate.Date)
        {
            if(lastEntry.EntryType == TimeEntryType.ClockOut)
                return new ()
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "You already clocked out today."
                };

            entryType = lastEntry.EntryType switch
            {
                TimeEntryType.ClockIn => TimeEntryType.BreakStart,
                TimeEntryType.BreakStart => TimeEntryType.BreakEnd,
                TimeEntryType.BreakEnd => TimeEntryType.ClockOut,
                _ => throw new ArgumentOutOfRangeException(nameof(lastEntry.EntryType), lastEntry.EntryType, null)
            };
        }

        var entity = new TimeEntry
        {
            EntryDate = currentDate,
            EntryType = entryType,
            WorkDate = DateOnly.FromDateTime(currentDate.DateTime.ToLocalTime())
        };

        var result = await service.AddAsync(entity);
        await repository.SaveChangesAsync();

        if (result is null)
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An error occurred while creating the time entry."
            };
        var response = mapper.Map<TimeEntryResponseViewModel>(result);
        return new()
        {
            Code = HttpStatusCode.Created,
            Data = response
        };
    }

    public async Task<Response<TimeEntryResponseViewModel>> CreateAsync(DateTime date)
    {
        var currentDate = date.ToUniversalTime();
        var dateEntry = await repository.GetByDateAsync(DateOnly.FromDateTime(date));
        var entryType = TimeEntryType.ClockIn;
        var lastEntry = dateEntry.OrderByDescending(x => x.EntryDate).FirstOrDefault();

        if (lastEntry is not null)
        {
            if (lastEntry.EntryType == TimeEntryType.ClockOut)
                return new()
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "You already clocked out today."
                };

            entryType = lastEntry.EntryType switch
            {
                TimeEntryType.ClockIn => TimeEntryType.BreakStart,
                TimeEntryType.BreakStart => TimeEntryType.BreakEnd,
                TimeEntryType.BreakEnd => TimeEntryType.ClockOut,
                _ => throw new ArgumentOutOfRangeException(nameof(lastEntry.EntryType), lastEntry.EntryType, null)
            };
        }

        var entity = new TimeEntry
        {
            EntryDate = currentDate,
            EntryType = entryType,
            WorkDate = DateOnly.FromDateTime(currentDate.ToLocalTime())
        };

        var result = await service.AddAsync(entity);
        await repository.SaveChangesAsync();

        if (result is null)
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An error occurred while creating the time entry."
            };
        var response = mapper.Map<TimeEntryResponseViewModel>(result);
        return new()
        {
            Code = HttpStatusCode.Created,
            Data = response
        };
    }

    public async Task<Response<GetDurationResponseViewModel>> GetDurationAsync(DateOnly date)
    {
        var entries = await repository.GetByDateAsync(date);
        if (entries is null || !entries.Any())
            return new()
            {
                Code = HttpStatusCode.BadRequest,
                Message = "No time entries found for the specified date."
            };

        var clockIn = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockIn)?.EntryDate;
        var breakStart = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakStart)?.EntryDate;
        var breakEnd = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakEnd)?.EntryDate;
        var clockOut = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockOut)?.EntryDate;

        double preBreakDuration = 0;
        double postBreakDuration = 0;
        if (breakStart is not null && clockIn is not null)
            preBreakDuration = (breakStart - clockIn).Value.TotalSeconds;

        if (clockOut is not null && breakEnd is not null)
            postBreakDuration = (clockOut - breakEnd).Value.TotalSeconds;

        var totalDuration = preBreakDuration + postBreakDuration;
        var totalDurationTimeSpan = TimeSpan.FromSeconds(totalDuration);
        var totalDurationFormatted =
            $"{totalDurationTimeSpan.Hours:D2}:" +
            $"{totalDurationTimeSpan.Minutes:D2}:" +
            $"{totalDurationTimeSpan.Seconds:D2}";

        var expectedDuration = TimeSpan.FromHours(8);
        var completedWorkTime = expectedDuration.TotalSeconds - totalDuration;
        var remainingWorkTime = expectedDuration.TotalSeconds - totalDuration;
        var remainingWorkTimeFormatted = TimeSpan.FromSeconds(remainingWorkTime).ToString(@"hh\:mm\:ss");

        TimeSpan completedWorkTimeAt = TimeSpan.Zero;
        if (clockOut is not null && breakEnd is not null)
            completedWorkTimeAt = clockOut.Value.LocalDateTime.AddSeconds(completedWorkTime).TimeOfDay;
        else if (clockOut is null && breakEnd is not null)
            completedWorkTimeAt = breakEnd.Value.LocalDateTime.AddSeconds(completedWorkTime).TimeOfDay;
        else if (clockIn is not null && breakStart is null)
            completedWorkTimeAt = clockIn.Value.LocalDateTime.AddSeconds(completedWorkTime).TimeOfDay;
        var completedWorkTimeAtFormatted = completedWorkTimeAt.ToString(@"hh\:mm\:ss");

        var clockInFormatted = clockIn?.LocalDateTime.ToString("HH:mm:ss") ?? "00:00:00";
        var clockOutFormatted = clockOut?.LocalDateTime.ToString("HH:mm:ss") ?? "00:00:00";
        var breakStartFormatted = breakStart?.LocalDateTime.ToString("HH:mm:ss") ?? "00:00:00";
        var breakEndFormatted = breakEnd?.LocalDateTime.ToString("HH:mm:ss") ?? "00:00:00";

        var response = new GetDurationResponseViewModel(clockInFormatted,
            breakStartFormatted, breakEndFormatted, clockOutFormatted,
            totalDurationFormatted, remainingWorkTimeFormatted, completedWorkTimeAtFormatted);

        return new()
        {
            Code = HttpStatusCode.OK,
            Data = response
        };
    }

    public async Task<Response<GetPeriodDurationResponseViewModel>> GetPeriodDurationAsync(DateOnly startDate, DateOnly endDate)
    {
        var entries = await repository.GetByDateRangeAsync(startDate, endDate);
        if (entries is null || !entries.Any())
            return new()
            {
                Code = HttpStatusCode.BadRequest,
                Message = "No time entries found for the period."
            };

        double totalWorkedTimeHours = 0;

        var workingDays = entries.Select(e => e.WorkDate).Distinct().ToList();

        foreach (var day in workingDays) 
        {
            var clockIn = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockIn && x.WorkDate == day)?.EntryDate;
            var breakStart = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakStart && x.WorkDate == day)?.EntryDate;
            var breakEnd = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakEnd && x.WorkDate == day)?.EntryDate;
            var clockOut = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockOut && x.WorkDate == day)?.EntryDate;

            double preBreakDuration = 0;
            double postBreakDuration = 0;
            if (breakStart is not null && clockIn is not null)
                preBreakDuration = (breakStart - clockIn).Value.TotalHours;

            if (clockOut is not null && breakEnd is not null)
                postBreakDuration = (clockOut - breakEnd).Value.TotalHours;

            var totalDuration = preBreakDuration + postBreakDuration;
            totalWorkedTimeHours += totalDuration;
        }

        var startDateTime = startDate.ToDateTime(new TimeOnly(0, 0, 0));
        var endDateTime = endDate.ToDateTime(new TimeOnly(23, 59, 59));

        var expectedWorkTimeTimeSpan = TimeSpan.FromHours((endDateTime - startDateTime).TotalDays * 8);
        var nonWorkingDays = await nonWorkingDayRepository.GetByDateRangeAsync(startDate, endDate);
        expectedWorkTimeTimeSpan -= TimeSpan.FromHours(nonWorkingDays.Count() * 8);

        var expectedWorkTimelHours = expectedWorkTimeTimeSpan.TotalHours;        

        return new()
        {
            Code = HttpStatusCode.OK,
            Data = new(totalWorkedTimeHours, expectedWorkTimelHours)
        };
    }

    public async Task<Response<GetTodaysDurationResponseViewModel>> GetTodaysDurationAsync()
    {
        var currentDate = DateOnly.FromDateTime(DateTime.Now);
        var entries = await repository.GetByDateAsync(currentDate);
        if (entries is null || !entries.Any())
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(0,0,0, "00:00:00")
            };

        var clockIn = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockIn)?.EntryDate;
        var breakStart = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakStart)?.EntryDate;
        var breakEnd = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakEnd)?.EntryDate;
        var clockOut = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockOut)?.EntryDate;

        var preBreakDuration = (breakStart - clockIn)?.TotalSeconds;
        var postBreakDuration = (clockOut - breakEnd)?.TotalSeconds;
        var totalDuration = preBreakDuration + postBreakDuration;
        var totalDurationTimeSpan = TimeSpan.FromSeconds(totalDuration??0);
        var totalDurationFormatted = 
            $"{totalDurationTimeSpan.Hours:D2}:" +
            $"{totalDurationTimeSpan.Minutes:D2}:" +
            $"{totalDurationTimeSpan.Seconds:D2}";

        var response = new GetTodaysDurationResponseViewModel(totalDurationTimeSpan.Hours,
            totalDurationTimeSpan.Minutes,
            totalDurationTimeSpan.Seconds,
            totalDurationFormatted);

        return new() { Code = HttpStatusCode.OK, Data = response };
    }

    public async Task<Response<GetWorkScheduleResponseViewModel>> GetWorkScheduleByDateAsync(DateOnly date)
    {
        const string clockFormat = "HH:mm";
        var entries = await repository.GetByDateAsync(date);
        var currentDate = DateTime.Now;

        if (entries is null || !entries.Any())
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new (ClockIn: "09:00",
                            BreakStart: "12:00",
                            BreakEnd: "13:00",
                            ClockOut: "18:00")
            };

        var clockIn = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockIn)?.EntryDate.LocalDateTime;
        var breakStart = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakStart)?.EntryDate.LocalDateTime;
        var breakEnd = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.BreakEnd)?.EntryDate.LocalDateTime;
        var clockOut = entries.FirstOrDefault(x => x.EntryType == TimeEntryType.ClockOut)?.EntryDate.LocalDateTime;

        if (clockIn != null && breakStart != null && breakEnd != null && clockOut != null)
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        if (clockIn != null && breakStart != null && breakEnd != null && clockOut == null)
        {
            var preBreakDuration = (breakStart - clockIn).Value;
            var postBreakDuration = 8 - preBreakDuration.TotalHours;
            clockOut = AdjustSeconds(breakEnd.Value.AddHours(postBreakDuration));
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }
        if (clockIn != null && breakStart != null && breakEnd == null && clockOut == null)
        {
            var preBreakDuration = (breakStart - clockIn).Value;
            breakEnd = AdjustSeconds(breakStart.Value.AddHours(1));
            breakEnd = breakEnd < currentDate ? currentDate : breakEnd;
            breakEnd = AdjustSeconds(breakEnd.Value);
            var postBreakDuration = 8 - preBreakDuration.TotalHours;
            clockOut = AdjustSeconds(breakEnd.Value.AddHours(postBreakDuration));
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }
        if (clockIn != null && breakStart == null && breakEnd == null && clockOut == null)
        {
            breakStart = AdjustSeconds(clockIn.Value.AddHours(3));
            breakStart = breakStart < currentDate ? currentDate : breakStart;
            breakStart = AdjustSeconds(breakStart.Value);
            var preBreakDuration = (breakStart - clockIn).Value;
            var postBreakDuration = 8 - preBreakDuration.TotalHours;
            breakEnd = AdjustSeconds(breakStart.Value.AddHours(1));
            clockOut = AdjustSeconds(breakEnd.Value.AddHours(postBreakDuration));
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }
        if (clockIn == null && breakStart == null && breakEnd == null && clockOut == null)
        {
            clockIn = date.ToDateTime(new TimeOnly(9, 0, 0));
            clockIn = clockIn < currentDate ? currentDate : clockIn;
            clockIn = AdjustSeconds(clockIn.Value);
            breakStart = AdjustSeconds(clockIn.Value.AddHours(3));
            var preBreakDuration = (breakStart - clockIn).Value;
            var postBreakDuration = 8 - preBreakDuration.TotalHours;
            breakEnd = AdjustSeconds(breakStart.Value.AddHours(1));
            clockOut = AdjustSeconds(breakEnd.Value.AddHours(postBreakDuration));
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }
        if (clockIn == null && breakStart == null && breakEnd != null && clockOut == null)
        {
            breakStart = AdjustSeconds(breakEnd.Value.AddHours(-1));
            clockIn = AdjustSeconds(breakStart.Value.AddHours(-3));
            clockOut = AdjustSeconds(breakEnd.Value.AddHours(5));
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }
        if (clockIn == null && breakStart != null && breakEnd == null && clockOut == null)
        {
            clockIn = AdjustSeconds(breakStart.Value.AddHours(-3));
            breakEnd = AdjustSeconds(breakStart.Value.AddHours(1));
            clockOut = AdjustSeconds(breakStart.Value.AddHours(5));

            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }
        if (clockIn == null && breakStart != null && breakEnd != null && clockOut == null)
        {
            clockIn = AdjustSeconds(breakStart.Value.AddHours(-3));
            clockOut = AdjustSeconds(breakEnd.Value.AddHours(5));
            clockIn = AdjustSeconds(clockIn.Value);
            clockOut = AdjustSeconds(clockOut.Value);
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }
        if (clockIn == null && breakStart != null && breakEnd != null && clockOut != null)
        {
            var postBreakDuration = (clockOut - breakEnd);
            var preBreakDuration = 8 - postBreakDuration.Value.TotalHours;
            clockIn = AdjustSeconds(breakStart.Value.AddHours(-3));
            clockIn = AdjustSeconds(clockIn.Value);
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = new(ClockIn: clockIn.Value.ToString(clockFormat),
                            BreakStart: breakStart.Value.ToString(clockFormat),
                            BreakEnd: breakEnd.Value.ToString(clockFormat),
                            ClockOut: clockOut.Value.ToString(clockFormat))
            };
        }

        return new()
        {
            Code = HttpStatusCode.BadRequest,
            Message = "Unable to determine work schedule."
        };
    }

    private static DateTime AdjustSeconds(DateTime time)
    {
        if (time.Second > 30)
            return time.AddSeconds(60 - time.Second);
        return time.AddSeconds(-1 * time.Second);
    }
}
