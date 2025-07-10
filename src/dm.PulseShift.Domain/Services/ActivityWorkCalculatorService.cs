using dm.PulseShift.Domain.Dtos;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Domain.Services;

public class ActivityWorkCalculatorService(IActivityRepository activityRepository, ITimeEntryRepository timeEntryRepository) : IActivityWorkCalculatorService
{
    public async Task<TimeSpan> CalculateEffectiveWorkTimeAsync(Activity activity)
    {
        if (activity.ActivityPeriods is null || !activity.ActivityPeriods.Any(p => !p.IsDeleted))
        {
            return TimeSpan.Zero;
        }

        var activityStartDate = activity.FirstOverallStartDate;
        if (!activityStartDate.HasValue)
        {
            return TimeSpan.Zero;
        }

        // Use the current time for open-ended activities.
        var lastDate = activity.LastOverallEndDate ?? DateTime.Now;
        var timeEntriesFetchStart = activityStartDate.Value.Date;
        var timeEntries = await timeEntryRepository.GetEntriesByDateRangeOrderedAsync(timeEntriesFetchStart, lastDate);
        if (!timeEntries.Any())
        {
            var simpleDuration = TimeSpan.Zero;
            foreach (var period in activity.ActivityPeriods.Where(p => !p.IsDeleted))
            {
                DateTime effectiveEndDate = period.EndDate ?? lastDate;
                simpleDuration += effectiveEndDate - period.StartDate;
            }
            return simpleDuration;
        }

        var workIntervalsByDay = GetWorkIntervalsByDay(timeEntries);

        var totalDuration = TimeSpan.Zero;
        foreach (var period in activity.ActivityPeriods.Where(p => !p.IsDeleted))
        {
            totalDuration += CalculateEffectivePeriodDuration(period.StartDate, period.EndDate, workIntervalsByDay);
        }

        return totalDuration;
    }

    public async Task<WorkTimeCalculationDto> CalculateTotalEffectiveActivityTimeInRangeAsync(DateTime rangeStart, DateTime rangeEnd)
    {
        var timeEntriesTask = timeEntryRepository.GetEntriesByDateRangeOrderedAsync(rangeStart, rangeEnd);
        var activitiesTask = activityRepository.GetActivitiesIntersectingDateRangeAsync(rangeStart, rangeEnd, null);

        await Task.WhenAll(timeEntriesTask, activitiesTask);

        var timeEntries = await timeEntriesTask;
        var activities = await activitiesTask;

        if (!timeEntries.Any())
        {
            return new(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);
        }

        var workIntervalsByDay = GetWorkIntervalsByDay(timeEntries);

        var totalWorkFromEntriesDuration = TimeSpan.Zero;
        foreach (var interval in workIntervalsByDay.SelectMany(kvp => kvp.Value))
        {
            var intersectStart = interval.Start > rangeStart ? interval.Start : rangeStart;
            var intersectEnd = interval.End < rangeEnd ? interval.End : rangeEnd;

            if (intersectStart < intersectEnd)
            {
                totalWorkFromEntriesDuration += intersectEnd - intersectStart;
            }
        }

        var totalWorkCoveredByActivitiesDuration = TimeSpan.Zero;
        if (activities.Any())
        {
            foreach (var activity in activities)
            {
                foreach (var period in activity.ActivityPeriods.Where(p => !p.IsDeleted))
                {
                    var periodStartInRange = period.StartDate > rangeStart ? period.StartDate : rangeStart;
                    var periodEndInRange = (period.EndDate ?? rangeEnd) < rangeEnd ? (period.EndDate ?? rangeEnd) : rangeEnd;

                    if (periodStartInRange < periodEndInRange)
                    {
                        totalWorkCoveredByActivitiesDuration += CalculateEffectivePeriodDuration(
                            periodStartInRange,
                            periodEndInRange,
                            workIntervalsByDay);
                    }
                }
            }
        }

        var unaccountedWorkDuration = totalWorkFromEntriesDuration - totalWorkCoveredByActivitiesDuration;

        if (unaccountedWorkDuration < TimeSpan.Zero) unaccountedWorkDuration = TimeSpan.Zero;

        // The conversion to .TotalHours is removed to return TimeSpan directly.
        return new(
            totalWorkFromEntriesDuration,
            totalWorkCoveredByActivitiesDuration,
            unaccountedWorkDuration
        );
    }

    private TimeSpan CalculateEffectivePeriodDuration(
        DateTime periodStart,
        DateTime? periodEnd,
        IReadOnlyDictionary<DateOnly, List<(DateTime Start, DateTime End)>> workIntervalsByDay)
    {
        var effectiveDuration = TimeSpan.Zero;
        var periodActualEnd = periodEnd ?? DateTime.Now;

        // Iterate through each day the activity period might span.
        for (var date = periodStart.Date; date <= periodActualEnd.Date; date = date.AddDays(1))
        {
            var workDate = DateOnly.FromDateTime(date);
            if (!workIntervalsByDay.TryGetValue(workDate, out var workIntervals))
            {
                continue;
            }

            foreach (var (workStart, workEnd) in workIntervals)
            {
                // Find the intersection between the activity period and the work interval.
                var intersectStart = periodStart > workStart ? periodStart : workStart;
                var intersectEnd = periodActualEnd < workEnd ? periodActualEnd : workEnd;

                if (intersectStart < intersectEnd)
                {
                    effectiveDuration += intersectEnd - intersectStart;
                }
            }
        }

        return effectiveDuration;
    }

    private Dictionary<DateOnly, List<(DateTime Start, DateTime End)>> GetWorkIntervalsByDay(
        IEnumerable<TimeEntry> timeEntries)
    {
        return timeEntries
            .GroupBy(te => te.WorkDate)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var intervals = new List<(DateTime Start, DateTime End)>();
                    var entries = group.ToDictionary(te => te.EntryType, te => te.EntryDate);

                    entries.TryGetValue(TimeEntryType.ClockIn, out var clockIn);
                    entries.TryGetValue(TimeEntryType.BreakStart, out var breakStart);
                    entries.TryGetValue(TimeEntryType.BreakEnd, out var breakEnd);
                    entries.TryGetValue(TimeEntryType.ClockOut, out var clockOut);

                    // The end of the day is either the clock-out time or the current time if still running.
                    var dayEnd = clockOut != default ? clockOut : DateTime.Now;

                    if (clockIn != default)
                    {
                        // First work interval: from ClockIn to BreakStart (or to the end of the day if no break).
                        var firstIntervalEnd = breakStart != default ? breakStart : dayEnd;
                        if (clockIn < firstIntervalEnd)
                        {
                            intervals.Add((clockIn, firstIntervalEnd));
                        }

                        // Second work interval: from BreakEnd to ClockOut (only if a break exists).
                        if (breakEnd != default)
                        {
                            // The end is ClockOut if it exists, otherwise it's the general 'dayEnd' (i.e., 'now').
                            var secondIntervalEnd = clockOut != default ? clockOut : dayEnd;
                            if (breakEnd < secondIntervalEnd)
                            {
                                intervals.Add((breakEnd, secondIntervalEnd));
                            }
                        }
                    }

                    return intervals;
                });
    }
}