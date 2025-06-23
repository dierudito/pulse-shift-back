namespace dm.PulseShift.Application.ViewModels.Responses.Reports;

public record WorkSegmentReportViewModel(
    string StartTime,
    string? EndTime,
    decimal HoursWorked,
    decimal HoursCoveredByActivities,
    decimal Efficiency,
    string EntryType,
    IEnumerable<ActivityEventViewModel> Activities
);
