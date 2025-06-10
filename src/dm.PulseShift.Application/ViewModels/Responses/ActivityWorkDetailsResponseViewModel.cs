namespace dm.PulseShift.Application.ViewModels.Responses;

public record ActivityWorkDetailsResponseViewModel(
    Guid Id,
    string? Description,
    string CardCode,
    string? CardLink,
    string TotalWorkedHours,
    string? FirstOverallStartDate,
    string? LastOverallEndDate,
    string CreatedAt,
    string? UpdatedAt,
    bool IsCurrentlyActive,
    IEnumerable<ActivityPeriodResponseViewModel> Periods
);