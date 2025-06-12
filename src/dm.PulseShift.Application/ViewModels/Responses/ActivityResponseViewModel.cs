namespace dm.PulseShift.Application.ViewModels.Responses;

public class ActivityResponseViewModel
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string CardCode { get; set; }
    public string? CardLink { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IEnumerable<ActivityPeriodResponseViewModel> Periods { get; set; }
    public bool IsCurrentlyActive { get; set; }
}    