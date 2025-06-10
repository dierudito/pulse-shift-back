using dm.PulseShift.Domain.Entities.Base;

namespace dm.PulseShift.Domain.Entities;

public class Activity : Entity
{
    public string? Description { get; set; }
    public string CardCode { get; set; } = default!;
    public string? CardLink { get; set; }
    public DateTimeOffset? FirstOverallStartDate =>
    _activityPeriods.Where(p => !p.IsDeleted).Any()
        ? _activityPeriods.Where(p => !p.IsDeleted).Min(p => p.StartDate)
        : null;
    public DateTimeOffset? LastOverallEndDate =>
        _activityPeriods.Where(p => !p.IsDeleted && p.EndDate.HasValue).Any()
            ? _activityPeriods.Where(p => !p.IsDeleted && p.EndDate.HasValue).Max(p => p.EndDate)
            : null;

    private readonly List<ActivityPeriod> _activityPeriods = new();
    public IReadOnlyCollection<ActivityPeriod> ActivityPeriods => _activityPeriods.AsReadOnly();

    public ActivityPeriod? GetCurrentOpenPeriod() =>
        _activityPeriods.SingleOrDefault(p => !p.EndDate.HasValue);

    public bool IsFinished() => _activityPeriods.Count != 0 && _activityPeriods.All(p => p.EndDate.HasValue);
    public bool IsCurrentlyActive => GetCurrentOpenPeriod() != null;

    public ActivityPeriod StartInitialPeriod(DateTimeOffset startDate, Guid startAssociatedTimeEntryId)
    {
        if (_activityPeriods.Count != 0)
            throw new InvalidOperationException("Initial period can only be started once.");

        return AddActivityPeriod(startDate, startAssociatedTimeEntryId);
    }

    public ActivityPeriod StartNewPeriod(DateTimeOffset startDate, Guid startAssociatedTimeEntryId)
    {
        var currentOpenPeriod = GetCurrentOpenPeriod();
        if (currentOpenPeriod != null)
            throw new InvalidOperationException("Cannot start a new period while another is currently open.");

        if (!_activityPeriods.Any())
            throw new InvalidOperationException("Cannot start a new period. The initial period must be created first (usually via activity creation).");

        return AddActivityPeriod(startDate, startAssociatedTimeEntryId);
    }

    public ActivityPeriod AddActivityPeriod(DateTimeOffset startDate, Guid startAssociatedTimeEntryId)
    {

        var newPeriod = new ActivityPeriod
        {
            ActivityId = Id,
            StartDate = startDate,
            AssociatedStartTimeEntryId = startAssociatedTimeEntryId
        };
        _activityPeriods.Add(newPeriod);
        UpdatedAt = DateTimeOffset.UtcNow;
        return newPeriod;
    }

    public ActivityPeriod FinishCurrentPeriod(DateTimeOffset endDate, Guid endAssociatedTimeEntryId)
    {
        var currentOpenPeriod = GetCurrentOpenPeriod();
        if (currentOpenPeriod == null)
            throw new InvalidOperationException("No open activity period to finish.");

        currentOpenPeriod.FinishPeriod(endDate, endAssociatedTimeEntryId);
        UpdatedAt = DateTimeOffset.UtcNow;
        return currentOpenPeriod;
    }

    public ActivityPeriod AddHistoricalPeriod(DateTimeOffset startDate, Guid startAssociatedTimeEntryId, 
        DateTimeOffset endDate, Guid endAssociatedTimeEntryId)
    {
        if (startDate >= endDate)
        {
            throw new ArgumentException("Start date must be before end date for a historical period.");
        }

        foreach (var existingPeriod in _activityPeriods.Where(p => !p.IsDeleted))
        {
            if ((existingPeriod.StartDate <= startDate && existingPeriod.EndDate > startDate) ||
                (existingPeriod.StartDate < endDate && existingPeriod.EndDate >= endDate))
            {
                throw new InvalidOperationException("The new historical period overlaps with an existing period for this activity.");
            }
        }

        var newPeriod = new ActivityPeriod
        {
            ActivityId = Id,
            StartDate = startDate,
            EndDate = endDate,
            AssociatedStartTimeEntryId = startAssociatedTimeEntryId,
            AssociatedEndTimeEntryId = endAssociatedTimeEntryId
        };
        _activityPeriods.Add(newPeriod);
        UpdatedAt = DateTimeOffset.UtcNow;
        return newPeriod;
    }
}