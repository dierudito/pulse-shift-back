using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Reports;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;
using System.Globalization;

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
        throw new Exception("This method is not implemented yet.");
    }
}
//        try
//        {
//            if (request.StartDate >= request.EndDate)
//            {
//                return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.BadRequest, Message = "Start date must be before end date." };
//            }

//            var queryStartUtc = request.StartDate.ToUniversalTime();
//            var queryEndUtc = request.EndDate.ToUniversalTime();
//            DateTimeOffset effectiveRangeEndQuery = Min(queryEndUtc, DateTimeOffset.UtcNow.ToUniversalTime().AddMinutes(1));


//            // 1. Obter totais gerais
//            WorkTimeCalculationResult overallTotals = await workCalculatorService.CalculateTotalWorkAndActivityTimeInRangeAsync(queryStartUtc, queryEndUtc);

//            string formattedTotalWorkHoursFromEntries = ((decimal)overallTotals.TotalWorkHoursFromEntries.TotalHours).ToString("F2", PtBrCulture);
//            string formattedTotalWorkCoveredByActivities = ((decimal)overallTotals.TotalWorkCoveredByActivities.TotalHours).ToString("F2", PtBrCulture);
//            string efficiency = "0,00";
//            if (overallTotals.TotalWorkHoursFromEntries.TotalSeconds > 0)
//            {
//                decimal efficiencyValue = ((decimal)overallTotals.TotalWorkCoveredByActivities.TotalSeconds / (decimal)overallTotals.TotalWorkHoursFromEntries.TotalSeconds) * 100;
//                efficiency = efficiencyValue.ToString("F2", PtBrCulture);
//            }

//            var allTimeEntriesInRange = (await timeEntryRepository.GetEntriesByDateRangeOrderedAsync(queryStartUtc, effectiveRangeEndQuery)).ToList();
//            var allActivitiesInRange = await activityRepository.GetActivitiesIntersectingDateRangeAsync(queryStartUtc, effectiveRangeEndQuery);
//            var allActivityPeriodsInRange = allActivitiesInRange
//                .SelectMany(a => a.ActivityPeriods.Where(p => !p.IsDeleted))
//                .ToList();

//            var timeEntriesByWorkDate = allTimeEntries
//                .GroupBy(te => DateOnly.FromDateTime(te.EntryDate.UtcDateTime.Date)) // Agrupar por data UTC
//                .ToDictionary(g => g.Key, g => g.OrderBy(te => te.EntryDate).ToList());

//            var dailyReportViewModels = new List<DailyTimeEntryReportViewModel>();

//            // 3. Iterar dia a dia no intervalo da consulta
//            for (DateTimeOffset currentDayIterator = queryStartUtc.Date; currentDayIterator.Date <= queryEndUtc.Date; currentDayIterator = currentDayIterator.AddDays(1))
//            {
//                DateOnly currentWorkDate = DateOnly.FromDateTime(currentDayIterator.UtcDateTime.Date);
//                DateTime currentSaoPauloDate = TimeZoneInfo.ConvertTimeFromUtc(currentDayIterator.UtcDateTime, TimeZoneHelper.SaoPauloTimeZone);

//                if (!timeEntriesByWorkDate.TryGetValue(currentWorkDate, out var dailyTimeEntriesForThisDay))
//                {
//                    // Adicionar dia sem registros de ponto, se desejado, ou pular
//                    // dailyReportViewModels.Add(new DailyTimeEntryReportViewModel(currentSaoPauloDate.ToString(DateFormat, PtBrCulture), "0,00", []));
//                    continue;
//                }

//                List<(DateTimeOffset Start, DateTimeOffset End)> workSegmentsForThisDayUtc = GetWorkSegmentsForDay(dailyTimeEntriesForThisDay);
//                TimeSpan dailyHoursWorkedFromEntries = TimeSpan.Zero;
//                var workPeriodReportViewModels = new List<WorkSegmentReportViewModel>();

//                foreach (var segmentUtc in workSegmentsForThisDayUtc)
//                {
//                    // Clipa o segmento de trabalho do dia pelo intervalo geral da query
//                    DateTimeOffset clippedSegmentStartUtc = Max(segmentUtc.Start, queryStartUtc);
//                    DateTimeOffset clippedSegmentEndUtc = Min(segmentUtc.End, queryEndUtc);

//                    if (clippedSegmentStartUtc >= clippedSegmentEndUtc) continue;

//                    dailyHoursWorkedFromEntries += (clippedSegmentEndUtc - clippedSegmentStartUtc);

//                    var activityEventsInSegment = new List<ActivityEventViewModel>();
//                    foreach (var activityPeriod in allActivityPeriodsInRange)
//                    {
//                        Activity parentActivity = allActivitiesInRange.First(a => a.Id == activityPeriod.ActivityId); // Encontra a atividade pai

//                        DateTimeOffset periodStartUtc = activityPeriod.StartDate.ToUniversalTime();
//                        DateTimeOffset periodEndUtc = (activityPeriod.EndDate ?? effectiveRangeEndQuery).ToUniversalTime(); // Usa effectiveRangeEndQuery para períodos ativos

//                        // Interseção do período da atividade com este segmento de trabalho (já clipado)
//                        DateTimeOffset activityOverlapStartUtc = Max(clippedSegmentStartUtc, periodStartUtc);
//                        DateTimeOffset activityOverlapEndUtc = Min(clippedSegmentEndUtc, periodEndUtc);

//                        if (activityOverlapStartUtc < activityOverlapEndUtc) // Se houver sobreposição
//                        {
//                            // Converter para hora de São Paulo para a string "WorkingPeriod"
//                            DateTime activityOverlapStartSp = TimeZoneInfo.ConvertTimeFromUtc(activityOverlapStartUtc.UtcDateTime, TimeZoneHelper.SaoPauloTimeZone);
//                            DateTime activityOverlapEndSp = TimeZoneInfo.ConvertTimeFromUtc(activityOverlapEndUtc.UtcDateTime, TimeZoneHelper.SaoPauloTimeZone);

//                            string workingPeriodStr = FormatActivityWorkingPeriod(activityPeriod, clippedSegmentStartUtc, clippedSegmentEndUtc, activityOverlapStartSp, activityOverlapEndSp);

//                            activityEventsInSegment.Add(new ActivityEventViewModel(
//                                parentActivity.Id,
//                                parentActivity.CardCode,
//                                $"{parentActivity.CardCode} - {parentActivity.Description}".TrimEnd(new[] { ' ', '-' }), // DisplayName
//                                workingPeriodStr
//                            ));
//                        }
//                    }

//                    DateTime segmentStartSp = TimeZoneInfo.ConvertTimeFromUtc(clippedSegmentStartUtc.UtcDateTime, TimeZoneHelper.SaoPauloTimeZone);
//                    DateTime segmentEndSp = TimeZoneInfo.ConvertTimeFromUtc(clippedSegmentEndUtc.UtcDateTime, TimeZoneHelper.SaoPauloTimeZone);

//                    // Lógica para EndTime "" se o ClockOut original estiver faltando para este segmento
//                    string endTimeStr = (segmentUtc.End == dailyTimeEntriesForThisDay.LastOrDefault(te => te.EntryType == TimeEntryType.ClockOut)?.EntryDate)
//                                        ? segmentEndSp.ToString(TimeFormat, PtBrCulture)
//                                        : (IsSegmentEffectivelyOpen(segmentUtc, dailyTimeEntriesForThisDay) ? "" : segmentEndSp.ToString(TimeFormat, PtBrCulture));


//                    workPeriodReportViewModels.Add(new WorkSegmentReportViewModel(
//                        segmentStartSp.ToString(TimeFormat, PtBrCulture),
//                        endTimeStr,
//                        activityEventsInSegment.OrderBy(a => a.WorkingPeriod).ToList() // Ordenar atividades por seu tempo de ocorrência
//                    ));
//                }

//                dailyReportViewModels.Add(new DailyTimeEntryReportViewModel(
//                    currentSaoPauloDate.ToString(DateFormat, PtBrCulture),
//                    ((decimal)dailyHoursWorkedFromEntries.TotalHours).ToString("F2", PtBrCulture),
//                    workPeriodReportViewModels
//                ));
//            }

//            var responseData = new PeriodReportResponseViewModel(
//                formattedTotalWorkHoursFromEntries,
//                formattedTotalWorkCoveredByActivities,
//                efficiency + "%", // Adiciona o símbolo de porcentagem
//                TimeZoneInfo.ConvertTimeFromUtc(queryStartUtc.UtcDateTime, TimeZoneHelper.SaoPauloTimeZone).ToString(DateFormat, PtBrCulture),
//                TimeZoneInfo.ConvertTimeFromUtc(queryEndUtc.UtcDateTime, TimeZoneHelper.SaoPauloTimeZone).ToString(DateFormat, PtBrCulture),
//                dailyReportViewModels.OrderBy(d => DateTime.ParseExact(d.WorkingDay, DateFormat, PtBrCulture)).ToList() // Ordenar dias
//            );

//            return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.OK, Data = responseData };
//        }
//        catch (Exception ex)
//        {
//            // TODO: Logar a exceção ex
//            return new Response<PeriodReportResponseViewModel> { Code = HttpStatusCode.InternalServerError, Message = $"An error occurred: {ex.Message}" };
//        }
//    }

//    // Reutilizar de ActivityWorkCalculatorService ou definir aqui
//    private List<(DateTimeOffset Start, DateTimeOffset End)> GetWorkSegmentsForDay(List<TimeEntry> dailyTimeEntries)
//    {
//        // ... (Implementação de GetWorkSegmentsForDay como fornecida anteriormente, garantindo que ela retorne segmentos UTC) ...
//        // Certifique-se que as EntryDate dos TimeEntry já estão em UTC ou são convertidas antes de usar.
//        var segments = new List<(DateTimeOffset Start, DateTimeOffset End)>();
//        if (dailyTimeEntries == null || !dailyTimeEntries.Any()) return segments;

//        var clockIn = dailyTimeEntries.FirstOrDefault(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockIn);
//        var clockOut = dailyTimeEntries.Where(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockOut).OrderByDescending(te => te.EntryDate).FirstOrDefault();

//        if (clockIn == null) return segments;

//        DateTimeOffset currentSegmentStart = clockIn.EntryDate.ToUniversalTime(); // Garante UTC
//        DateTimeOffset dayEndBoundaryForCalc = (clockOut?.EntryDate ?? clockIn.EntryDate.Date.AddDays(1).AddTicks(-1)).ToUniversalTime(); // Garante UTC
//        if (clockOut != null && clockOut.EntryDate.UtcDateTime.Date != clockIn.EntryDate.UtcDateTime.Date)
//        {
//            dayEndBoundaryForCalc = clockIn.EntryDate.UtcDateTime.Date.AddDays(1).AddTicks(-1);
//        }

//        var relevantBreaks = dailyTimeEntries
//            .Where(te => !te.IsDeleted && (te.EntryType == TimeEntryType.BreakStart || te.EntryType == TimeEntryType.BreakEnd) && te.EntryDate >= clockIn.EntryDate && (clockOut == null || te.EntryDate <= clockOut.EntryDate))
//            .OrderBy(te => te.EntryDate)
//            .ToList();

//        foreach (var breakEntry in relevantBreaks)
//        {
//            var breakEntryDateUtc = breakEntry.EntryDate.ToUniversalTime();
//            if (breakEntry.EntryType == TimeEntryType.BreakStart && breakEntryDateUtc > currentSegmentStart)
//            {
//                segments.Add((currentSegmentStart, Min(breakEntryDateUtc, dayEndBoundaryForCalc)));
//                currentSegmentStart = dayEndBoundaryForCalc;
//            }
//            else if (breakEntry.EntryType == TimeEntryType.BreakEnd && breakEntryDateUtc > currentSegmentStart)
//            {
//                currentSegmentStart = breakEntryDateUtc;
//            }
//        }

//        if (currentSegmentStart < dayEndBoundaryForCalc)
//        {
//            segments.Add((currentSegmentStart, dayEndBoundaryForCalc));
//        }

//        DateTimeOffset absoluteDayStart = clockIn.EntryDate.ToUniversalTime();
//        DateTimeOffset absoluteDayEnd = (clockOut?.EntryDate ?? clockIn.EntryDate.Date.AddDays(1).AddTicks(-1)).ToUniversalTime();

//        return segments
//            .Select(s => (Start: Max(s.Start, absoluteDayStart), End: Min(s.End, absoluteDayEnd)))
//            .Where(s => s.End > s.Start)
//            .ToList();
//    }

//    private bool IsSegmentEffectivelyOpen(ValueTuple<DateTimeOffset, DateTimeOffset> segmentUtc, List<TimeEntry> dailyTimeEntriesForThisDay)
//    {
//        // Verifica se o EndTime do segmento corresponde a um ClockOut real
//        var clockOut = dailyTimeEntriesForThisDay
//            .Where(te => !te.IsDeleted && te.EntryType == TimeEntryType.ClockOut)
//            .OrderByDescending(te => te.EntryDate)
//            .FirstOrDefault();

//        if (segmentUtc.Item1.UtcDateTime.Date == segmentUtc.Item2.UtcDateTime.Date && // mesmo dia
//            clockOut != null && segmentUtc.Item2 == clockOut.EntryDate.ToUniversalTime())
//        {
//            return false; // Segmento fechado por um ClockOut
//        }
//        if (segmentUtc.Item2.TimeOfDay == TimeSpan.FromDays(1).Add(TimeSpan.FromTicks(-1))) // Termina no fim do dia
//        {
//            return clockOut == null || clockOut.EntryDate.ToUniversalTime() < segmentUtc.Item2; // Aberto se não houver clockout ou se o clockout for antes do fim do dia
//        }
//        // Poderia ter lógica mais complexa aqui, mas se o segmento termina e não é o ClockOut, algo está faltando.
//        // Se o último evento do dia para este segmento não é um clockout que o fecha.
//        var lastEventForSegment = dailyTimeEntriesForThisDay
//                                .Where(te => te.EntryDate.ToUniversalTime() <= segmentUtc.Item2)
//                                .OrderByDescending(te => te.EntryDate)
//                                .FirstOrDefault();
//        return lastEventForSegment != null && lastEventForSegment.EntryType != TimeEntryType.ClockOut;

//    }


//    private string FormatActivityWorkingPeriod(ActivityPeriod activityPeriod, DateTimeOffset workSegmentStartUtc, DateTimeOffset workSegmentEndUtc, DateTime activityOverlapStartSp, DateTime activityOverlapEndSp)
//    {
//        // Converte limites do activityPeriod para UTC para comparação precisa de eventos
//        DateTimeOffset activityStartUtc = activityPeriod.StartDate.ToUniversalTime();
//        DateTimeOffset? activityEndUtcNullable = activityPeriod.EndDate?.ToUniversalTime();

//        // O 'activityOverlapStartSp' e 'activityOverlapEndSp' já são a interseção em SP
//        bool startedInOrAtSegmentBoundary = activityStartUtc >= workSegmentStartUtc && activityStartUtc < workSegmentEndUtc;
//        bool endedInOrAtSegmentBoundary = activityEndUtcNullable.HasValue && activityEndUtcNullable.Value > workSegmentStartUtc && activityEndUtcNullable.Value <= workSegmentEndUtc;

//        if (startedInOrAtSegmentBoundary && endedInOrAtSegmentBoundary)
//        {
//            // Se iniciou e terminou dentro do overlap SP (que é o overlap com o workSegment)
//            return $"{activityOverlapStartSp.ToString(TimeFormat, PtBrCulture)} - {activityOverlapEndSp.ToString(TimeFormat, PtBrCulture)}";
//        }
//        if (startedInOrAtSegmentBoundary) // Começou dentro do segmento de trabalho (ou no seu início) e termina depois (ou está ativa)
//        {
//            return activityOverlapStartSp.ToString(TimeFormat, PtBrCulture);
//        }
//        if (endedInOrAtSegmentBoundary) // Terminou dentro do segmento de trabalho (ou no seu fim) e começou antes
//        {
//            return activityOverlapEndSp.ToString(TimeFormat, PtBrCulture);
//        }
//        // Se cruzou todo o overlap (começou antes do overlap E terminou depois do overlap OU está ativa)
//        if (activityStartUtc < activityOverlapStartSp.ToUniversalTime(TimeZoneHelper.SaoPauloTimeZone) &&
//            (!activityEndUtcNullable.HasValue || activityEndUtcNullable.Value > activityOverlapEndSp.ToUniversalTime(TimeZoneHelper.SaoPauloTimeZone)))
//        {
//            return $"{activityOverlapStartSp.ToString(TimeFormat, PtBrCulture)} - {activityOverlapEndSp.ToString(TimeFormat, PtBrCulture)}";
//        }
//        // Caso de fallback ou se a lógica acima não cobrir perfeitamente, pode retornar o intervalo de overlap
//        return $"{activityOverlapStartSp.ToString(TimeFormat, PtBrCulture)} - {activityOverlapEndSp.ToString(TimeFormat, PtBrCulture)}";
//    }


//    private DateTimeOffset Max(DateTimeOffset d1, DateTimeOffset d2) => d1 > d2 ? d1 : d2;
//    private DateTimeOffset Min(DateTimeOffset d1, DateTimeOffset d2) => d1 < d2 ? d1 : d2;
//}

//// Extensão para converter DateTime para DateTimeOffset UTC (exemplo)
//public static class DateTimeExtensions
//{
//    public static DateTimeOffset ToUniversalTime(this DateTime dateTime, TimeZoneInfo sourceTimeZone)
//    {
//        if (dateTime.Kind == DateTimeKind.Unspecified)
//        {
//            return TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone);
//        }
//        return dateTime.ToUniversalTime(); // Usa a conversão padrão se Kind for Local ou Utc
//    }
//}