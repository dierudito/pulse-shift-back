namespace dm.PulseShift.Application.ViewModels.Responses;

public record GetPeriodDurationResponseViewModel(
    double totalWorkedTimeHours,
    double expectedWorkTimeHours);