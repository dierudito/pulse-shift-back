namespace dm.PulseShift.Application.ViewModels.Responses;

public record GetTodaysDurationResponseViewModel(
    int Hour,
    int Minute,
    int Second,
    string FormattedTime);