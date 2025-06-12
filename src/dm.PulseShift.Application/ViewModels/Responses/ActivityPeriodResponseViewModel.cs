namespace dm.PulseShift.Application.ViewModels.Responses;

public class ActivityPeriodResponseViewModel
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid StartAssociatedTimeEntryId { get; set; }
    public Guid? EndAssociatedTimeEntryId { get; set; }
    public DateTime CreatedAt { get; set; }
}