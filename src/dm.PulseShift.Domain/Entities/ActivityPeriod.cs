using dm.PulseShift.Domain.Entities.Base;

namespace dm.PulseShift.Domain.Entities;

public class ActivityPeriod : Entity
{
    public Guid ActivityId { get; set; }
    public virtual Activity Activity { get; set; } = null!;
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid AssociatedStartTimeEntryId { get; set; }
    public virtual TimeEntry StartTimeEntry { get; set; } = null!;
    public Guid? AssociatedEndTimeEntryId { get; set; }
    public virtual TimeEntry? EndTimeEntry { get; set; }

    public void FinishPeriod(DateTimeOffset endDate, Guid endAssociatedTimeEntryId)
    {
        if (EndDate.HasValue)
            throw new InvalidOperationException("This activity period has already been finished.");
        if (endDate == default)
            throw new ArgumentException("End date must be provided.", nameof(endDate));
        if (endDate < StartDate)
            throw new ArgumentException("End date cannot be earlier than start date for the period.", nameof(endDate));
        if (endAssociatedTimeEntryId == Guid.Empty)
            throw new ArgumentException("EndAssociatedTimeEntryId must be provided for finishing the period.", nameof(endAssociatedTimeEntryId));

        EndDate = endDate;
        AssociatedEndTimeEntryId = endAssociatedTimeEntryId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
