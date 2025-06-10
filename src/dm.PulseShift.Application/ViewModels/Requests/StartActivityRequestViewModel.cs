namespace dm.PulseShift.Application.ViewModels.Requests;

public record StartActivityRequestViewModel(
    DateTimeOffset? StartDate = null
);