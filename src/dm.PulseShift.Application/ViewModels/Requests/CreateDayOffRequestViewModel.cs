namespace dm.PulseShift.Application.ViewModels.Requests;

public record CreateDayOffRequestViewModel(DateOnly Date, string? Description);