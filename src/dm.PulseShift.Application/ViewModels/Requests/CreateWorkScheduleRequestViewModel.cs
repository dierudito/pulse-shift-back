namespace dm.PulseShift.Application.ViewModels.Requests;
public record CreateWorkScheduleRequestViewModel(
    TimeSpan WorkStartTime,
    TimeSpan LunchBreakStartTime,
    TimeSpan LunchBreatEndTime,
    TimeSpan WorkEndTime);
