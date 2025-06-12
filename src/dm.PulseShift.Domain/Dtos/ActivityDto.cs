namespace dm.PulseShift.Domain.Dtos;

public record ActivityDto(
    Guid? Id,
    string CardCode,
    string Description,
    string? CardLink,
    DateTime StartDate,
    DateTime? EndDate = null,
    Guid? AssociatedStartTimeEntryId = default,
    Guid? AssociatedEndTimeEntryId = default
);