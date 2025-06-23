using System.ComponentModel.DataAnnotations;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record PeriodReportRequestViewModel(
    [Required] DateOnly StartDate,
    [Required] DateOnly EndDate
);