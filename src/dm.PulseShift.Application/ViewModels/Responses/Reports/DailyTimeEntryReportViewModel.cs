namespace dm.PulseShift.Application.ViewModels.Responses.Reports;

public record DailyTimeEntryReportViewModel(
    DateOnly WorkingDay,
    decimal HoursWorked,
    decimal HoursCoveredByActivities,
    decimal Efficiency,
    string? ClockInTime,
    string? ClockOutTime,
    IEnumerable<WorkSegmentReportViewModel> WorkPeriods
);
