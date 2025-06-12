using System.ComponentModel.DataAnnotations;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record PeriodReportRequestViewModel(
    [Required] DateTime StartDate,
    [Required] DateTime EndDate
);