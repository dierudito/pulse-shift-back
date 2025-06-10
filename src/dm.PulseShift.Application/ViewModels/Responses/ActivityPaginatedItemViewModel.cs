namespace dm.PulseShift.Application.ViewModels.Responses;

public record ActivityPaginatedItemViewModel(
    Guid Id,
    string CardCode,
    string? Description,
    string TotalWorkedHours, // Formatado como "00,00"
    bool IsCurrentlyActive,
    string? FormattedLastOverallEndDate // Adicionado para visualização e verificação da ordenação
);