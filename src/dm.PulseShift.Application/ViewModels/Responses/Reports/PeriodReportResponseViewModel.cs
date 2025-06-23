namespace dm.PulseShift.Application.ViewModels.Responses.Reports;

public record PeriodReportResponseViewModel(
    decimal TotalWorkHoursFromEntries,
    decimal TotalWorkCoveredByActivities,
    decimal Efficiency,
    DateOnly QueryStartDate,
    DateOnly QueryEndDate,
    IEnumerable<DailyTimeEntryReportViewModel> DailyReports
);