using System.ComponentModel.DataAnnotations;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record WorkHoursQueryRequestViewModel(
    [Required] DateTime StartDate,
    [Required] DateTime EndDate
);