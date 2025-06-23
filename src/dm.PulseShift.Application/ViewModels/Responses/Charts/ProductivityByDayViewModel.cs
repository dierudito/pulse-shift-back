namespace dm.PulseShift.Application.ViewModels.Responses.Charts;

public record ProductivityByDayViewModel(
    string DayOfWeek,
    double DurationHours
);