using System.ComponentModel.DataAnnotations;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record WorkHoursQueryRequestViewModel(
    [Required] DateTimeOffset StartDate,
    [Required] DateTimeOffset EndDate
);