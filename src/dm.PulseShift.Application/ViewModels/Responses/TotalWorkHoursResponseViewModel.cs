namespace dm.PulseShift.Application.ViewModels.Responses;

public record TotalWorkHoursResponseViewModel(
    string TotalWorkHoursFromEntries,
    string TotalWorkCoveredByActivities,
    string QueryStartDate,
    string QueryEndDate
);