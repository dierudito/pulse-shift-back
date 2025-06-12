using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Reports;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;
using dm.PulseShift.Infra.CrossCutting.Shared.Helpers;
using System.Globalization;
using System.Net;

namespace dm.PulseShift.Application.AppServices;


public class ReportAppService(
    IActivityRepository activityRepository,
    ITimeEntryRepository timeEntryRepository,
    IActivityWorkCalculatorService workCalculatorService) : IReportAppService
{
    private static readonly CultureInfo PtBrCulture = CultureInfo.GetCultureInfo("pt-BR");
    private const string TimeFormat = "HH:mm";
    private const string DateFormat = "dd/MM/yyyy";

    public async Task<Response<PeriodReportResponseViewModel>> GetPeriodReportAsync(PeriodReportRequestViewModel request)
    {
        try
        {
            if (request.StartDate >= request.EndDate)
            {
                return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.BadRequest, Message = "Start date must be before end date." };
            }

            var effectiveRangeEndQuery = Min(request.EndDate, DateTime.Now.AddMinutes(1));


            // 1. Obter totais gerais
            var overallTotals = await workCalculatorService.CalculateTotalEffectiveActivityTimeInRangeAsync(request.StartDate, request.EndDate);

            string formattedTotalWorkHoursFromEntries = ((decimal)overallTotals.TotalWorkHoursFromEntries.TotalHours).ToString("F2", PtBrCulture);
            string formattedTotalWorkCoveredByActivities = ((decimal)overallTotals.TotalWorkCoveredByActivities.TotalHours).ToString("F2", PtBrCulture);
            string efficiency = "0,00";
            if (overallTotals.TotalWorkHoursFromEntries.TotalSeconds > 0)
            {
                decimal efficiencyValue = ((decimal)overallTotals.TotalWorkCoveredByActivities.TotalSeconds / (decimal)overallTotals.TotalWorkHoursFromEntries.TotalSeconds) * 100;
                efficiency = efficiencyValue.ToString("F2", PtBrCulture);
            }

            var allTimeEntriesInRange = (await timeEntryRepository.GetEntriesByDateRangeOrderedAsync(request.StartDate, effectiveRangeEndQuery)).ToList();
            var allActivitiesInRange = await activityRepository.GetActivitiesIntersectingDateRangeAsync(request.StartDate, effectiveRangeEndQuery);
            var allActivityPeriodsInRange = allActivitiesInRange
                .SelectMany(a => a.ActivityPeriods.Where(p => !p.IsDeleted))
                .ToList();

            var timeEntriesByWorkDate = allTimeEntriesInRange
                .GroupBy(te => DateOnly.FromDateTime(te.EntryDate))
                .ToDictionary(g => g.Key, g => g.OrderBy(te => te.EntryDate).ToList());

            var dailyReportViewModels = new List<DailyTimeEntryReportViewModel>();

            // 3. Iterar dia a dia no intervalo da consulta
            for (DateTime currentDayIterator = request.StartDate.Date; currentDayIterator.Date <= request.EndDate.Date; currentDayIterator = currentDayIterator.AddDays(1))
            {
                DateOnly currentWorkDate = DateOnly.FromDateTime(currentDayIterator.Date);

                if (!timeEntriesByWorkDate.TryGetValue(currentWorkDate, out var dailyTimeEntriesForThisDay))
                {
                    continue;
                }

                List<(DateTime Start, DateTime End)> workSegmentsForThisDay = GetWorkSegmentsForDay(dailyTimeEntriesForThisDay);
                TimeSpan dailyHoursWorkedFromEntries = TimeSpan.Zero;
                var workPeriodReportViewModels = new List<WorkSegmentReportViewModel>();

                foreach (var segment in workSegmentsForThisDay)
                {
                    // Clipa o segmento de trabalho do dia pelo intervalo geral da query
                    DateTime clippedSegmentStart = Max(segment.Start, request.StartDate);
                    DateTime clippedSegmentEnd = Min(segment.End, request.EndDate);

                    if (clippedSegmentStart >= clippedSegmentEnd) continue;

                    dailyHoursWorkedFromEntries += (clippedSegmentEnd - clippedSegmentStart);

                    var activityEventsInSegment = new List<ActivityEventViewModel>();
                    foreach (var activityPeriod in allActivityPeriodsInRange)
                    {
                        Activity parentActivity = allActivitiesInRange.First(a => a.Id == activityPeriod.ActivityId); // Encontra a atividade pai

                        var periodEnd = activityPeriod.EndDate ?? effectiveRangeEndQuery;

                        // Interseção do período da atividade com este segmento de trabalho (já clipado)
                        DateTime activityOverlapStart = Max(clippedSegmentStart, activityPeriod.StartDate);
                        DateTime activityOverlapEnd = Min(clippedSegmentEnd, periodEnd);

                        if (activityOverlapStart < activityOverlapEnd) // Se houver sobreposição
                        {
                            string workingPeriodStr = FormatActivityWorkingPeriod(
                                activityPeriod, clippedSegmentStart, clippedSegmentEnd,
                                activityOverlapStart, activityOverlapEnd);

                            activityEventsInSegment.Add(new ActivityEventViewModel(
                                parentActivity.Id,
                                parentActivity.CardCode,
                                $"{parentActivity.CardCode} - {parentActivity.Description}".TrimEnd([' ', '-']), // DisplayName
                                workingPeriodStr
                            ));
                        }
                    }

                    string endTimeStr = (segment.End == dailyTimeEntriesForThisDay.LastOrDefault(te => te.EntryType == TimeEntryType.ClockOut)?.EntryDate)
                                        ? clippedSegmentEnd.ToString(TimeFormat, PtBrCulture)
                                        : (IsSegmentEffectivelyOpen(segment, dailyTimeEntriesForThisDay) ? "" : clippedSegmentEnd.ToString(TimeFormat, PtBrCulture));

                    workPeriodReportViewModels.Add(new WorkSegmentReportViewModel(
                                            clippedSegmentStart.ToString(TimeFormat, PtBrCulture),
                                            endTimeStr,
                                            [.. activityEventsInSegment.OrderBy(a => a.WorkingPeriod)]
                                        ));
                }

                dailyReportViewModels.Add(new DailyTimeEntryReportViewModel(
                    currentDayIterator.ToString(DateFormat, PtBrCulture),
                    ((decimal)dailyHoursWorkedFromEntries.TotalHours).ToString("F2", PtBrCulture),
                    workPeriodReportViewModels
                ));
            }

            var responseData = new PeriodReportResponseViewModel(
                formattedTotalWorkHoursFromEntries,
                formattedTotalWorkCoveredByActivities,
                efficiency + "%", // Adiciona o símbolo de porcentagem
                request.StartDate.ToString(DateFormat, PtBrCulture),
                request.EndDate.ToString(DateFormat, PtBrCulture),
                [.. dailyReportViewModels.OrderBy(d => DateTime.ParseExact(d.WorkingDay, DateFormat, PtBrCulture))] // Ordenar dias
            );

            return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.OK, Data = responseData };
        }
        catch (Exception ex)
        {
            // TODO: Logar a exceção ex
            return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.InternalServerError, Message = $"An error occurred: {ex.Message}" };
        }
    }

    // Reutilizar de ActivityWorkCalculatorService ou definir aqui
    private List<(DateTime Start, DateTime End)> GetWorkSegmentsForDay(List<TimeEntry> dailyTimeEntries)
    {
        var segments = new List<(DateTime Start, DateTime End)>();
        if (dailyTimeEntries == null || dailyTimeEntries.Count == 0) return segments;

        var clockIn = dailyTimeEntries.FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockIn);
        var clockOut = dailyTimeEntries.Where(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockOut).OrderByDescending(te => te.EntryDate).FirstOrDefault();

        if (clockIn == null) return segments;

        var currentSegmentStart = clockIn.EntryDate;
        var dayEndBoundaryForCalc = clockOut?.EntryDate ?? clockIn.EntryDate.Date.AddDays(1).AddTicks(-1);

        if (clockOut != null && clockOut.EntryDate.Date != clockIn.EntryDate.Date)
        {
            dayEndBoundaryForCalc = clockIn.EntryDate.Date.AddDays(1).AddTicks(-1);
        }

        var relevantBreaks = dailyTimeEntries
            .Where(te => !te.IsDeleted &&
                         (te.EntryType == TimeEntryType.BreakStart || te.EntryType == TimeEntryType.BreakEnd) &&
                          te.EntryDate >= clockIn.EntryDate &&
                         (clockOut == null || te.EntryDate <= clockOut.EntryDate))
            .OrderBy(te => te.EntryDate)
            .ToList();

        foreach (var breakEntry in relevantBreaks)
        {
            if (breakEntry.EntryType == TimeEntryType.BreakStart && breakEntry.EntryDate > currentSegmentStart)
            {
                segments.Add((currentSegmentStart, Min(breakEntry.EntryDate, dayEndBoundaryForCalc)));
                currentSegmentStart = dayEndBoundaryForCalc;
            }
            else if (breakEntry.EntryType == TimeEntryType.BreakEnd && breakEntry.EntryDate > currentSegmentStart)
            {
                currentSegmentStart = breakEntry.EntryDate;
            }
        }

        if (currentSegmentStart < dayEndBoundaryForCalc)
        {
            segments.Add((currentSegmentStart, dayEndBoundaryForCalc));
        }

        var absoluteDayStart = clockIn.EntryDate;
        var absoluteDayEnd = (clockOut?.EntryDate ?? clockIn.EntryDate.Date.AddDays(1).AddTicks(-1));

        return [.. segments
            .Select(s => (Start: Max(s.Start, absoluteDayStart), End: Min(s.End, absoluteDayEnd)))
            .Where(s => s.End > s.Start)];
    }

    private static bool IsSegmentEffectivelyOpen(ValueTuple<DateTime, DateTime> segment, List<TimeEntry> dailyTimeEntriesForThisDay)
    {
        // Verifica se o EndTime do segmento corresponde a um ClockOut real
        var clockOut = dailyTimeEntriesForThisDay
            .Where(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockOut)
            .OrderByDescending(te => te.EntryDate)
            .FirstOrDefault();

        if (segment.Item1.Date == segment.Item2.Date && // mesmo dia
            clockOut != null && segment.Item2 == clockOut.EntryDate)
        {
            return false; // Segmento fechado por um ClockOut
        }
        if (segment.Item2.TimeOfDay == TimeSpan.FromDays(1).Add(TimeSpan.FromTicks(-1))) // Termina no fim do dia
        {
            return clockOut == null || clockOut.EntryDate < segment.Item2; // Aberto se não houver clockout ou se o clockout for antes do fim do dia
        }
        // Poderia ter lógica mais complexa aqui, mas se o segmento termina e não é o ClockOut, algo está faltando.
        // Se o último evento do dia para este segmento não é um clockout que o fecha.
        var lastEventForSegment = dailyTimeEntriesForThisDay
                                .Where(te => te.EntryDate <= segment.Item2)
                                .OrderByDescending(te => te.EntryDate)
                                .FirstOrDefault();
        return lastEventForSegment != null && lastEventForSegment.EntryType != TimeEntryType.ClockOut;

    }


    private string FormatActivityWorkingPeriod(ActivityPeriod activityPeriod, DateTime workSegmentStart, DateTime workSegmentEnd, DateTime activityOverlapStart, DateTime activityOverlapEnd)
    {
        // O 'activityOverlapStart' e 'activityOverlapEnd' já são a interseção em SP
        var startedInOrAtSegmentBoundary =
            activityPeriod.StartDate >= workSegmentStart &&
            activityPeriod.StartDate < workSegmentEnd;
        var endedInOrAtSegmentBoundary =
            activityPeriod.EndDate.HasValue &&
            activityPeriod.EndDate.Value > workSegmentStart &&
            activityPeriod.EndDate.Value <= workSegmentEnd;

        if (startedInOrAtSegmentBoundary && endedInOrAtSegmentBoundary)
        {
            // Se iniciou e terminou dentro do overlap SP (que é o overlap com o workSegment)
            return $"{activityOverlapStart.ToString(TimeFormat, PtBrCulture)} - {activityOverlapEnd.ToString(TimeFormat, PtBrCulture)}";
        }
        if (startedInOrAtSegmentBoundary) // Começou dentro do segmento de trabalho (ou no seu início) e termina depois (ou está ativa)
        {
            return activityOverlapStart.ToString(TimeFormat, PtBrCulture);
        }
        if (endedInOrAtSegmentBoundary) // Terminou dentro do segmento de trabalho (ou no seu fim) e começou antes
        {
            return activityOverlapEnd.ToString(TimeFormat, PtBrCulture);
        }
        // Se cruzou todo o overlap (começou antes do overlap E terminou depois do overlap OU está ativa)
        if (activityPeriod.StartDate < activityOverlapStart &&
            (!activityPeriod.EndDate.HasValue || activityPeriod.EndDate.Value > activityOverlapEnd))
        {
            return $"{activityOverlapStart.ToString(TimeFormat, PtBrCulture)} - {activityOverlapEnd.ToString(TimeFormat, PtBrCulture)}";
        }
        // Caso de fallback ou se a lógica acima não cobrir perfeitamente, pode retornar o intervalo de overlap
        return $"{activityOverlapStart.ToString(TimeFormat, PtBrCulture)} - {activityOverlapEnd.ToString(TimeFormat, PtBrCulture)}";
    }

    private static DateTime Max(DateTime d1, DateTime d2) => d1 > d2 ? d1 : d2;
    private static DateTime Min(DateTime d1, DateTime d2) => d1 < d2 ? d1 : d2;
}