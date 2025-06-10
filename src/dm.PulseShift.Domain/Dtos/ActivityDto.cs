namespace dm.PulseShift.Domain.Dtos;

public record ActivityDto(
    Guid? Id,
    string CardCode,
    string Description,
    string? CardLink,
    DateTimeOffset StartDate,
    DateTimeOffset? EndDate = null,
    Guid? AssociatedStartTimeEntryId = default,
    Guid? AssociatedEndTimeEntryId = default
);