namespace dm.PulseShift.Application.ViewModels.Responses;

public class ActivityPeriodResponseViewModel
{
    public Guid Id { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public Guid StartAssociatedTimeEntryId { get; set; }
    public Guid? EndAssociatedTimeEntryId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}