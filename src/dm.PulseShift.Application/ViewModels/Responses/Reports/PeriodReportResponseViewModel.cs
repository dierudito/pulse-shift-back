namespace dm.PulseShift.Application.ViewModels.Responses.Reports;

public record PeriodReportResponseViewModel(
    string TotalWorkHoursFromEntries,
    string TotalWorkCoveredByActivities,
    string Efficiency,
    string QueryStartDate,
    string QueryEndDate,
    IEnumerable<DailyTimeEntryReportViewModel> DailyReports
);