namespace dm.PulseShift.Application.ViewModels.Responses;

public record GetWorkScheduleResponseViewModel(string ClockIn, string BreakStart, string BreakEnd, string ClockOut);