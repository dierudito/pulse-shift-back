using System.ComponentModel.DataAnnotations;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record PeriodReportRequestViewModel(
    [Required] DateTimeOffset StartDate,
    [Required] DateTimeOffset EndDate
);