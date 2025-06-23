using dm.PulseShift.Domain.Dtos;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Domain.Services;

public class ActivityWorkCalculatorService(IActivityRepository activityRepository, ITimeEntryRepository timeEntryRepository) : IActivityWorkCalculatorService
{
    public async Task<TimeSpan> CalculateEffectiveWorkTimeAsync(Activity targetActivity)
    {
        if (targetActivity == null || !targetActivity.ActivityPeriods.Any(p => !p.IsDeleted))
        {
            return TimeSpan.Zero;
        }

        var nonDeletedPeriods = targetActivity.ActivityPeriods.Where(p => !p.IsDeleted).ToList();

        var overallMinDate = targetActivity.FirstOverallStartDate!.Value;
        var effectiveQueryEndDate = targetActivity.IsCurrentlyActive
            ? DateTime.Now
            : nonDeletedPeriods.Where(p => p.EndDate.HasValue).Select(p => p.EndDate.Value).DefaultIfEmpty(overallMinDate).Max();
        if (effectiveQueryEndDate < overallMinDate && !targetActivity.IsCurrentlyActive) effectiveQueryEndDate = overallMinDate;


        var allTimeEntries = (await timeEntryRepository.GetEntriesByDateRangeOrderedAsync(overallMinDate, effectiveQueryEndDate)).ToList();
        var firstGeneralStartActivity = targetActivity.FirstGeneralStartActivity;
        var lastGeneralEndActivity = targetActivity.LastGeneralEndActivity ?? firstGeneralStartActivity;
        var startTimeEntry = await timeEntryRepository.GetByIdAsync(firstGeneralStartActivity!.AssociatedStartTimeEntryId);
        var endTimeEntry = lastGeneralEndActivity?.AssociatedEndTimeEntryId != null
            ? await timeEntryRepository.GetByIdAsync(lastGeneralEndActivity.AssociatedEndTimeEntryId.Value)
            : null;

        allTimeEntries.Add(startTimeEntry!);
        if (endTimeEntry != null && !allTimeEntries.Any(te => te.Id == endTimeEntry.Id))
        {
            allTimeEntries.Add(endTimeEntry);
        }
        var timeEntriesByWorkDate = allTimeEntries
            .GroupBy(te => DateOnly.FromDateTime(te.EntryDate.Date))
            .ToDictionary(g => g.Key, g => g.OrderBy(te => te.EntryDate).ToList());

        TimeSpan totalEffectiveWorkTime = TimeSpan.Zero;

        foreach (var targetPeriod in nonDeletedPeriods.OrderBy(p => p.StartDate))
        {
            var periodActualStart = targetPeriod.StartDate;
            var periodActualEnd = targetPeriod.EndDate ?? DateTime.Now; // Se ativa, até agora

            for (var currentDateIterator = periodActualStart.Date; currentDateIterator.Date <= periodActualEnd.Date; currentDateIterator = currentDateIterator.AddDays(1))
            {
                DateOnly workDate = DateOnly.FromDateTime(currentDateIterator.Date);
                if (!timeEntriesByWorkDate.TryGetValue(workDate, out var dailyTimeEntries))
                {
                    continue;
                }

                List<(DateTime Start, DateTime End)> workSegments = GetWorkSegmentsForDay(dailyTimeEntries);

                foreach (var (Start, End) in workSegments)
                {
                    var effectiveStart = Max(periodActualStart, Start);
                    var effectiveEnd = Min(periodActualEnd, End);

                    if (effectiveStart < effectiveEnd)
                    {
                        // Não há mais necessidade de subtrair 'interruptionsInSegment' de outras atividades
                        // porque a nova regra de negócio impede a sobreposição.
                        totalEffectiveWorkTime += (effectiveEnd - effectiveStart);
                    }
                }
            }
        }
        return totalEffectiveWorkTime;
    }

    public async Task<WorkTimeCalculationDto> CalculateTotalEffectiveActivityTimeInRangeAsync(DateTime rangeStart, DateTime rangeEnd)
    {

        var effectiveRangeEndQuery = Min(rangeEnd, DateTime.Now.AddMinutes(1));

        var allTimeEntries = (await timeEntryRepository.GetEntriesByDateRangeOrderedAsync(rangeStart, effectiveRangeEndQuery)).ToList();

        var allActivitiesInRange = await activityRepository.GetActivitiesIntersectingDateRangeAsync(rangeStart, effectiveRangeEndQuery);
        var allActivityPeriods = allActivitiesInRange
            .SelectMany(a => a.ActivityPeriods.Where(p => !p.IsDeleted))
            .ToList();

        var timeEntriesByWorkDate = allTimeEntries
            .GroupBy(te => DateOnly.FromDateTime(te.EntryDate))
            .ToDictionary(g => g.Key, g => g.OrderBy(te => te.EntryDate).ToList());

        var totalWorkHoursFromEntries = TimeSpan.Zero;
        var totalWorkCoveredByActivities = TimeSpan.Zero;

        // Loop pelos dias no intervalo da consulta
        for (var currentDateIterator = rangeStart.Date; currentDateIterator.Date <= rangeEnd.Date; currentDateIterator = currentDateIterator.AddDays(1))
        {
            var workDate = DateOnly.FromDateTime(currentDateIterator);
            if (!timeEntriesByWorkDate.TryGetValue(workDate, out var dailyTimeEntries))
            {
                continue; // Sem registros de ponto para este dia
            }

            List<(DateTime Start, DateTime End)> workSegmentsOnDay = GetWorkSegmentsForDay(dailyTimeEntries);

            foreach (var (Start, End) in workSegmentsOnDay)
            {
                // Considera o segmento de trabalho dentro do intervalo da consulta
                var clippedWorkSegmentStart = Max(Start, rangeStart);
                var clippedWorkSegmentEnd = Min(End, rangeEnd);

                if (clippedWorkSegmentStart >= clippedWorkSegmentEnd) continue;

                // Adiciona a duração do segmento de trabalho válido ao total de horas de expediente
                totalWorkHoursFromEntries += (clippedWorkSegmentEnd - clippedWorkSegmentStart);

                // Calcula o tempo coberto por atividades DENTRO deste segmento de trabalho específico
                if (allActivityPeriods.Count != 0) // Só processa atividades se houver alguma
                {
                    var activeSubSegmentsInThisWorkSegment = new List<(DateTime Start, DateTime End)>();
                    foreach (var activityPeriod in allActivityPeriods)
                    {
                        var periodStart = activityPeriod.StartDate;
                        var periodEnd = activityPeriod.EndDate ?? Min(DateTime.Now, rangeEnd);

                        // Interseção do período da atividade com o segmento de trabalho já clipado pela query
                        var overlapStart = Max(clippedWorkSegmentStart, periodStart);
                        var overlapEnd = Min(clippedWorkSegmentEnd, periodEnd);

                        if (overlapStart < overlapEnd)
                        {
                            activeSubSegmentsInThisWorkSegment.Add((overlapStart, overlapEnd));
                        }
                    }

                    var mergedActivityCoverage = MergeOverlappingIntervals(activeSubSegmentsInThisWorkSegment);
                    foreach (var mergedSegment in mergedActivityCoverage)
                    {
                        totalWorkCoveredByActivities += (mergedSegment.End - mergedSegment.Start);
                    }
                }
            }
        }
        return new(totalWorkHoursFromEntries, totalWorkCoveredByActivities);
    }

    private List<(DateTime Start, DateTime End)> GetWorkSegmentsForDay(List<TimeEntry> dailyTimeEntries)
    {
        var segments = new List<(DateTime Start, DateTime End)>();
        if (dailyTimeEntries == null || dailyTimeEntries.Count == 0) return segments;

        var clockIn = dailyTimeEntries.FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockIn);
        // Consider the last ClockOut of the day, relevant if there are multiple clock-in/out cycles (though less common for typical 4-point days)
        var clockOut = dailyTimeEntries.Where(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockOut).OrderByDescending(te => te.EntryDate).FirstOrDefault();


        if (clockIn == null) return segments;

        var currentSegmentStart = clockIn.EntryDate;
        // Default dayEndBoundary to end of the clockIn day if no clockOut, or actual clockOut time.
        // If clockOut is on a different day (unlikely for typical setup but possible), cap at end of clockIn's day for this iteration.
        var dayEndBoundaryForCalc = clockOut?.EntryDate ?? clockIn.EntryDate.Date.AddDays(1).AddTicks(-1);
        if (clockOut != null && clockOut.EntryDate.Date != clockIn.EntryDate.Date)
        {
            dayEndBoundaryForCalc = clockIn.EntryDate.Date.AddDays(1).AddTicks(-1); // Cap at end of current day being processed
        }


        var relevantBreaks = dailyTimeEntries
            .Where(te => !te.IsDeleted && (te.EntryType == TimeEntryType.BreakStart || te.EntryType == TimeEntryType.BreakEnd) && te.EntryDate >= clockIn.EntryDate && (clockOut == null || te.EntryDate <= clockOut.EntryDate))
            .OrderBy(te => te.EntryDate)
            .ToList();

        foreach (var breakEntry in relevantBreaks)
        {
            if (breakEntry.EntryType == TimeEntryType.BreakStart && breakEntry.EntryDate > currentSegmentStart)
            {
                segments.Add((currentSegmentStart, Min(breakEntry.EntryDate, dayEndBoundaryForCalc)));
                currentSegmentStart = dayEndBoundaryForCalc; // Assume break ends the current segment possibility until a BreakEnd
            }
            else if (breakEntry.EntryType == TimeEntryType.BreakEnd && breakEntry.EntryDate > currentSegmentStart)
            {
                currentSegmentStart = breakEntry.EntryDate; // Resume work from BreakEnd
            }
        }

        // Add the last segment from the last processed point to clock-out (or effective end of day)
        if (currentSegmentStart < dayEndBoundaryForCalc)
        {
            segments.Add((currentSegmentStart, dayEndBoundaryForCalc));
        }

        // Final filtering and ensuring segments are within the true overall ClockIn and ClockOut of the day
        var absoluteDayStart = clockIn.EntryDate;
        var absoluteDayEnd = clockOut?.EntryDate ?? clockIn.EntryDate.Date.AddDays(1).AddTicks(-1); // If no clockout, consider end of day

        return segments
            .Select(s => (Start: Max(s.Start, absoluteDayStart), End: Min(s.End, absoluteDayEnd)))
            .Where(s => s.End > s.Start)
            .ToList();
    }


    private List<(DateTime Start, DateTime End)> MergeOverlappingIntervals(List<(DateTime Start, DateTime End)> intervals)
    {
        if (intervals == null || !intervals.Any()) return new List<(DateTime Start, DateTime End)>();

        var sortedIntervals = intervals.OrderBy(i => i.Start).ToList();
        var merged = new LinkedList<(DateTime Start, DateTime End)>(); // Use LinkedList for efficient Last access/modification

        foreach (var current in sortedIntervals)
        {
            if (merged.Last == null || current.Start >= merged.Last.Value.End) // No overlap with the last merged interval or first interval
            {
                merged.AddLast(current);
            }
            else // Overlap or adjacent
            {
                var lastMergedNode = merged.Last;
                // Update the End of the last merged interval if current extends it
                if (current.End > lastMergedNode.Value.End)
                {
                    lastMergedNode.Value = (lastMergedNode.Value.Start, current.End);
                }
            }
        }
        return merged.ToList();
    }

    private DateTime Max(DateTime d1, DateTime d2) => d1 > d2 ? d1 : d2;
    private DateTime Min(DateTime d1, DateTime d2) => d1 < d2 ? d1 : d2;
}