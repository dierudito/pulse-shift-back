namespace dm.PulseShift.Application.ViewModels.Responses;
public record TimeEntryResponseViewModel(
    Guid Id,
    DateTime EntryDate,
    string EntryType,
    string? Description);

public record GetTimeEntriesPerDay(Guid Id, string EntryDate, string EntryType, string CreatedAt, string? UpdatedAt);