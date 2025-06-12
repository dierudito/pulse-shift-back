using System.ComponentModel.DataAnnotations;

namespace dm.PulseShift.Application.ViewModels.Requests;
public record CreateActivityRequestViewModel(
    [Required] string Description,
    [Required] string CardCode,
    string? CardLink,
    DateTime? StartDate = null
);