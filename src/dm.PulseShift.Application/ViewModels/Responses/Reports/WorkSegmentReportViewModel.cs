namespace dm.PulseShift.Application.ViewModels.Responses.Reports;

public record WorkSegmentReportViewModel(
    string StartTime,
    string EndTime,
    IEnumerable<ActivityEventViewModel> Activities
);
