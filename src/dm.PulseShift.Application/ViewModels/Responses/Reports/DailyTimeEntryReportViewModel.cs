namespace dm.PulseShift.Application.ViewModels.Responses.Reports;

public record DailyTimeEntryReportViewModel(
    string WorkingDay,
    string HoursWorked,
    IEnumerable<WorkSegmentReportViewModel> WorkPeriods
);
