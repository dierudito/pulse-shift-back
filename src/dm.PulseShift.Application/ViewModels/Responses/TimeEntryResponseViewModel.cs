namespace dm.PulseShift.Application.ViewModels.Responses;
public record TimeEntryResponseViewModel(
    Guid Id,
    DateTime EntryDate,
    string EntryType,
    string? Description);
