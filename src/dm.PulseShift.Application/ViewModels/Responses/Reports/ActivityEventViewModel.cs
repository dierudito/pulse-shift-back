namespace dm.PulseShift.Application.ViewModels.Responses.Reports;

public record ActivityEventViewModel(
    Guid Id,
    string CardCode,
    string DisplayName,
    string WorkingPeriod
);