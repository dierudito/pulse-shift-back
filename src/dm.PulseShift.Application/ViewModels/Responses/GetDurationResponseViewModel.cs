namespace dm.PulseShift.Application.ViewModels.Responses;

public record GetDurationResponseViewModel (
    string clockIn,
    string breakStart,
    string breakEnd,
    string clockOut,
    string totalWorkedTime,
    string remainingWorkTime,
    string completedWorkTimeAt);