namespace dm.PulseShift.Application.ViewModels.Requests;

public record FinishActivityRequestViewModel(
    DateTimeOffset? EndDate = null
);