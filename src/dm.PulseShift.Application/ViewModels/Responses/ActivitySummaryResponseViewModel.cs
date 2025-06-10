namespace dm.PulseShift.Application.ViewModels.Responses;

public record ActivitySummaryResponseViewModel (Guid Id, string CardCode, string? Description)
{
    private readonly string _displayDescription = string.IsNullOrWhiteSpace(Description) ? "" : $" - {Description}";
    public string DisplayName => $"{CardCode}{_displayDescription}";
}