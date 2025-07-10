using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Reports;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using System.Globalization;
using System.Net;

namespace dm.PulseShift.Application.AppServices;


public class ReportAppService(
    IActivityRepository activityRepository,
    ITimeEntryRepository timeEntryRepository) : IReportAppService
{
    private const string TimeFormat = "HH:mm";

    public async Task<Response<PeriodReportResponseViewModel>> GetPeriodReportAsync(PeriodReportRequestViewModel request)
    {
        try
        {
            var startDate = request.StartDate.ToDateTime(TimeOnly.MinValue);
            var endDate = request.EndDate.ToDateTime(TimeOnly.MaxValue);

            if (startDate >= endDate)
            {
                return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.BadRequest, Message = "Start date must be before end date." };
            }

            var effectiveRangeEndQuery = Min(endDate, DateTime.Now.AddMinutes(1));

            // 1. Obter todos os dados brutos relevantes uma única vez
            var allTimeEntriesInRange = (await timeEntryRepository.GetEntriesByDateRangeOrderedAsync(startDate, effectiveRangeEndQuery)).ToList();
            var allActivitiesInRange = await activityRepository.GetActivitiesIntersectingDateRangeAsync(startDate, effectiveRangeEndQuery);
            var allActivityPeriodsInRange = allActivitiesInRange
                .SelectMany(a => a.ActivityPeriods.Where(p => !p.IsDeleted))
                .OrderBy(a => a.StartDate)
                .ToList();

            var timeEntriesByWorkDate = allTimeEntriesInRange
                .GroupBy(te => DateOnly.FromDateTime(te.EntryDate))
                .ToDictionary(g => g.Key, g => g.OrderBy(te => te.EntryDate).ToList());

            var dailyReportViewModels = new List<DailyTimeEntryReportViewModel>();

            TimeSpan overallWorkedFromEntries = TimeSpan.Zero;
            TimeSpan overallCoveredByActivities = TimeSpan.Zero;

            // 2. Iterar dia a dia para construir o relatório detalhado
            for (DateTime currentDayIterator = startDate.Date; currentDayIterator.Date <= endDate.Date; currentDayIterator = currentDayIterator.AddDays(1))
            {
                var currentWorkDate = DateOnly.FromDateTime(currentDayIterator);
                if (!timeEntriesByWorkDate.TryGetValue(currentWorkDate, out var dailyTimeEntries))
                {
                    continue; // Pular dias sem registros de ponto
                }

                List<(DateTime Start, DateTime End)> workSegmentsForDay = GetWorkSegmentsForDay(dailyTimeEntries);
                TimeSpan dailyWorkedFromEntries = TimeSpan.Zero;
                TimeSpan dailyCoveredByActivities = TimeSpan.Zero;
                var workPeriodReportViewModels = new List<WorkSegmentReportViewModel>();
                var clockInTime = dailyTimeEntries
                    .FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockIn)?.EntryDate;
                var clockOutTime = dailyTimeEntries
                    .FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockOut)?.EntryDate;

                foreach (var (Start, End) in workSegmentsForDay)
                {
                    
                    var clippedSegmentStart = Max(Start, startDate);
                    var clippedSegmentEnd = Min(End, endDate);
                    clippedSegmentEnd = Min(DateTime.Now, clippedSegmentEnd); // Garantir que não ultrapasse o tempo atual
                    if (clippedSegmentStart >= clippedSegmentEnd) continue;

                    var segmentWorkDuration = clippedSegmentEnd - clippedSegmentStart;
                    dailyWorkedFromEntries += segmentWorkDuration;

                    // Calcula o tempo coberto por atividades para este segmento específico
                    var activityCoverageInSegment = CalculateActivityCoverageForSegment(clippedSegmentStart, clippedSegmentEnd, allActivityPeriodsInRange, effectiveRangeEndQuery);
                    dailyCoveredByActivities += activityCoverageInSegment;

                    // Popula as atividades para este segmento
                    var activitiesInSegment = GetActivitiesForSegment(clippedSegmentStart, clippedSegmentEnd, allActivityPeriodsInRange, allActivitiesInRange, effectiveRangeEndQuery);

                    workPeriodReportViewModels.Add(new WorkSegmentReportViewModel(
                        clippedSegmentStart.ToString("HH:mm"),
                        clippedSegmentEnd.ToString("HH:mm"),
                        ToDecimalHours(segmentWorkDuration),
                        ToDecimalHours(activityCoverageInSegment),
                        CalculateEfficiency(activityCoverageInSegment, segmentWorkDuration),
                        GetSegmentStartEntryType(Start, dailyTimeEntries),
                        activitiesInSegment
                    ));
                }

                if (workPeriodReportViewModels.Count != 0)
                {
                    overallWorkedFromEntries += dailyWorkedFromEntries;
                    overallCoveredByActivities += dailyCoveredByActivities;

                    dailyReportViewModels.Add(new DailyTimeEntryReportViewModel(
                        currentWorkDate,
                        ToDecimalHours(dailyWorkedFromEntries),
                        ToDecimalHours(dailyCoveredByActivities),
                        CalculateEfficiency(dailyCoveredByActivities, dailyWorkedFromEntries),
                        clockInTime?.ToString("HH:mm"),
                        clockOutTime?.ToString("HH:mm"),
                        workPeriodReportViewModels
                    ));
                }
            }

            var responseData = new PeriodReportResponseViewModel(
                ToDecimalHours(overallWorkedFromEntries),
                ToDecimalHours(overallCoveredByActivities),
                CalculateEfficiency(overallCoveredByActivities, overallWorkedFromEntries),
                request.StartDate,
                request.EndDate,
                dailyReportViewModels.OrderBy(d => d.WorkingDay).ToList()
            );

            return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.OK, Data = responseData };
        }
        catch (Exception ex)
        {
            // TODO: Logar a exceção ex
            return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.InternalServerError, Message = $"An error occurred: {ex.Message}" };
        }
    }

    private decimal ToDecimalHours(TimeSpan timeSpan) => Math.Round((decimal)timeSpan.TotalHours, 2);

    private decimal CalculateEfficiency(TimeSpan coveredTime, TimeSpan totalTime) =>
        totalTime.TotalSeconds > 0
            ? Math.Round(((decimal)coveredTime.TotalSeconds / (decimal)totalTime.TotalSeconds) * 100, 2)
            : 0;

    private TimeSpan CalculateActivityCoverageForSegment(DateTime segmentStart, DateTime segmentEnd, List<ActivityPeriod> allPeriods, DateTime effectiveRangeEnd)
    {
        var activeSubSegments = new List<(DateTime Start, DateTime End)>();
        foreach (var period in allPeriods)
        {
            var periodStart = period.StartDate;
            var periodEnd = period.EndDate ?? effectiveRangeEnd;
            var overlapStart = Max(segmentStart, periodStart);
            var overlapEnd = Min(segmentEnd, periodEnd);
            if (overlapStart < overlapEnd)
            {
                activeSubSegments.Add((overlapStart, overlapEnd));
            }
        }
        var mergedCoverage = MergeOverlappingIntervals(activeSubSegments);
        return TimeSpan.FromSeconds(mergedCoverage.Sum(s => (s.End - s.Start).TotalSeconds));
    }

    private static IEnumerable<ActivityEventViewModel> GetActivitiesForSegment(DateTime segmentStart, DateTime segmentEnd, List<ActivityPeriod> allPeriods, IEnumerable<Activity> allActivities, DateTime effectiveRangeEnd)
    {
        var activityEvents = new List<ActivityEventViewModel>();
        foreach (var period in allPeriods)
        {
            var periodStart = period.StartDate;
            var periodEnd = period.EndDate ?? effectiveRangeEnd;
            if (periodStart < segmentEnd && periodEnd > segmentStart) // Checa sobreposição
            {
                var parentActivity = allActivities.First(a => a.Id == period.ActivityId);
                activityEvents.Add(new ActivityEventViewModel(
                    parentActivity.Id,
                    parentActivity.CardCode,
                    $"{parentActivity.CardCode} - {parentActivity.Description}".TrimEnd([' ', '-']),
                    FormatActivityWorkingPeriod(period, segmentStart, segmentEnd)
                ));
            }
        }
        return activityEvents;
    }

    private static string FormatActivityWorkingPeriod(ActivityPeriod activityPeriod, DateTime workSegmentStart, DateTime workSegmentEnd)
    {
        var activityStart = activityPeriod.StartDate;
        var activityEnd = activityPeriod.EndDate;

        bool startedInSegment = activityStart >= workSegmentStart && activityStart < workSegmentEnd;
        bool endedInSegment = activityEnd.HasValue && activityEnd.Value > workSegmentStart && activityEnd.Value <= workSegmentEnd;

        // Se a atividade estava ativa durante todo o segmento de trabalho, mas não começou nem terminou nele.
        if (!startedInSegment && !endedInSegment)
        {
            return ""; // Retorna vazio conforme a nova regra
        }

        // Formata os horários para HH:mm
        string startTimeString = activityStart.ToString(TimeFormat, CultureInfo.InvariantCulture);
        string? endTimeString = activityEnd?.ToString(TimeFormat, CultureInfo.InvariantCulture);

        if (startedInSegment && endedInSegment) return $"{startTimeString} - {endTimeString}";
        if (startedInSegment) return startTimeString;
        if (endedInSegment) return endTimeString!;

        return ""; // Fallback
    }

    private string GetSegmentStartEntryType(DateTime segmentStart, List<TimeEntry> dailyEntries)
    {
        return dailyEntries.FirstOrDefault(e => e.EntryDate == segmentStart)?.EntryType.ToString() ?? "Unknown";
    }

    private static List<(DateTime Start, DateTime End)> GetWorkSegmentsForDay(List<TimeEntry> dailyTimeEntries)
    {
        var segments = new List<(DateTime Start, DateTime End)>();
        if (dailyTimeEntries == null || dailyTimeEntries.Count == 0) return segments;
        var clockIn =
            dailyTimeEntries.FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockIn);
        var clockOut =
            dailyTimeEntries
            .Where(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockOut)
            .OrderByDescending(te => te.EntryDate)
            .FirstOrDefault();
        if (clockIn == null) return segments;
        var currentSegmentStart = clockIn.EntryDate;
        var dayEndBoundaryForCalc = clockOut?.EntryDate ??
                                    clockIn.EntryDate.Date.AddDays(1).AddTicks(-1);
        if (clockOut != null && clockOut.EntryDate.Date != clockIn.EntryDate.Date)
            dayEndBoundaryForCalc = clockIn.EntryDate.Date.AddDays(1).AddTicks(-1);

        var breakStart = dailyTimeEntries.FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.BreakStart)?.EntryDate ?? dayEndBoundaryForCalc;
        var breakEnd = dailyTimeEntries.FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.BreakEnd)?.EntryDate ?? dayEndBoundaryForCalc;
        segments.Add((currentSegmentStart, breakStart));
        segments.Add((breakEnd, dayEndBoundaryForCalc));

        var absoluteDayStart = clockIn.EntryDate; 
        var absoluteDayEnd = (clockOut?.EntryDate ?? clockIn.EntryDate.Date.AddDays(1).AddTicks(-1));
        return [.. segments.Select(s => (Start: Max(s.Start, absoluteDayStart), End: Min(s.End, absoluteDayEnd))).Where(s => s.End > s.Start)];
    }
    private List<(DateTime Start, DateTime End)> MergeOverlappingIntervals(List<(DateTime Start, DateTime End)> intervals)
    {
        if (intervals == null || !intervals.Any()) return new List<(DateTime Start, DateTime End)>();
        var sortedIntervals = intervals.OrderBy(i => i.Start).ToList();
        var merged = new LinkedList<(DateTime Start, DateTime End)>();
        foreach (var current in sortedIntervals) { if (merged.Last == null || current.Start >= merged.Last.Value.End) { merged.AddLast(current); } else { var lastMergedNode = merged.Last; if (current.End > lastMergedNode.Value.End) { lastMergedNode.Value = (lastMergedNode.Value.Start, current.End); } } }
        return merged.ToList();
    }

    private static DateTime Max(DateTime d1, DateTime d2) => d1 > d2 ? d1 : d2;
    private static DateTime Min(DateTime d1, DateTime d2) => d1 < d2 ? d1 : d2;
}