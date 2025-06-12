using System.ComponentModel.DataAnnotations;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record CreateRetroactiveActivityPeriodRequestViewModel(
    [Required] string CardCode,
    [Required] DateTime StartDate,
    [Required] DateTime EndDate
);