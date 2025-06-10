namespace dm.PulseShift.Application.ViewModels.Responses.Base;

public record PaginatedResponseViewModel<T>(
    int PageNumber,
    int PageSize,
    int TotalPages,
    long TotalRecords,
    IEnumerable<T> Data);