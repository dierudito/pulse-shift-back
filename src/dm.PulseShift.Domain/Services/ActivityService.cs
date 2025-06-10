using dm.PulseShift.Domain.Dtos;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;

namespace dm.PulseShift.Domain.Services;

public class ActivityService(
    IActivityRepository activityRepository, 
    ITimeEntryRepository timeEntryRepository,
    IActivityPeriodRepository activityPeriodRepository) : IActivityService
{
    public async Task<(Activity Activity, ActivityPeriod InitialPeriod)> CreateActivityAsync(ActivityDto activityDto)
    {
        if (await activityRepository.DoesCardCodeExistAsync(activityDto.CardCode))
        {
            var existingActivity = await activityRepository.GetByCardCodeWithPeriodsAsync(activityDto.CardCode);
            return (existingActivity!, await StartActivityAsync(existingActivity!.Id, activityDto.StartDate));
        }
        var activity = new Activity
        {
            CardCode = activityDto.CardCode,
            Description = activityDto.Description,
            CardLink = activityDto.CardLink
        };
        var associatedTimeEntry = await GetAssociatedTimeEntryAsync(activityDto.StartDate);

        await FinishCurrentPeriod(activityDto.StartDate, associatedTimeEntry!.Id);
        var initialPeriod = activity.StartInitialPeriod(activityDto.StartDate, associatedTimeEntry!.Id);
        await activityRepository.AddAsync(activity);
        return (activity, initialPeriod);
    }

    public async Task<ActivityPeriod> StartActivityAsync(Guid activityId, DateTimeOffset startDate)
    {
        var activity = await activityRepository.GetByIdWithPeriodsAsync(activityId);

        if (activity == null || activity.IsDeleted)
        {
            throw new KeyNotFoundException($"Activity with Id '{activityId}' not found or is deleted.");
        }

        if (activity.GetCurrentOpenPeriod() != null)
        {
            throw new InvalidOperationException($"Activity with CardCode '{activity.CardCode}' already has an active period. Finish it before starting a new one.");
        }

        var startAssociatedTimeEntryId = await GetAssociatedTimeEntryAsync(startDate);
        await FinishCurrentPeriod(startDate, startAssociatedTimeEntryId!.Id);

        var newPeriod = activity.StartNewPeriod(startDate, startAssociatedTimeEntryId!.Id);

        await activityRepository.UpdateAsync(activity, activity.Id);
        await UpsertPeriodsAsync(activity);
        return newPeriod;
    }

    public async Task<ActivityPeriod> FinishCurrentActivityPeriodAsync(Guid activityId, DateTimeOffset endDate)
    {
        var activity = await activityRepository.GetByIdWithPeriodsAsync(activityId);
        if (activity == null || activity.IsDeleted)
        {
            throw new KeyNotFoundException($"Activity with Id '{activityId}' not found or is deleted.");
        }

        var endAssociatedTimeEntryId = await GetAssociatedTimeEntryAsync(endDate);
        var finishedPeriod = activity.FinishCurrentPeriod(endDate, endAssociatedTimeEntryId!.Id);

        await activityRepository.UpdateAsync(activity, activity.Id);
        await UpsertPeriodsAsync(activity);
        return finishedPeriod;
    }

    public async Task<ActivityPeriod> AddRetroactivePeriodAsync(string cardCode, DateTimeOffset startDate, DateTimeOffset endDate)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date.");
        if (startDate > DateTimeOffset.UtcNow || endDate > DateTimeOffset.UtcNow)
            throw new ArgumentException("Retroactive period dates cannot be in the future.");

        var activity = await activityRepository.GetByCardCodeWithPeriodsAsync(cardCode);
        if (activity == null || activity.IsDeleted)
        {
            throw new KeyNotFoundException($"Activity with CardCode '{cardCode}' not found or is deleted.");
        }

        var startIdForNewRetroPeriod = 
            (await GetAssociatedTimeEntryAsync(startDate))?.Id ?? 
             throw new InvalidOperationException($"No TimeEntry found near start date {startDate} for target activity.");
        var endIdForNewRetroPeriod = (
            await GetAssociatedTimeEntryAsync(endDate))?.Id ?? 
            throw new InvalidOperationException($"No TimeEntry found near end date {endDate} for target activity.");

        var otherActivitiesToAdjust = await activityRepository.GetActivitiesIntersectingDateRangeAsync(startDate, endDate, activity.Id);

        foreach (var otherActivity in otherActivitiesToAdjust)
        {
            var overlappingPeriodsOfOtherActivity = otherActivity.ActivityPeriods
                .Where(p => !p.IsDeleted && p.StartDate < endDate && (p.EndDate == null || p.EndDate > startDate))
                .ToList(); // ToList para evitar problemas de modificação durante iteração

            foreach (var existingPeriod in overlappingPeriodsOfOtherActivity)
            {
                var originalExistingPeriodEndDate = existingPeriod.EndDate ?? DateTimeOffset.MaxValue;
                Guid? originalExistingPeriodEndAssociatedId = existingPeriod.AssociatedEndTimeEntryId;

                var endIdForTruncatedPart = (
                    await GetAssociatedTimeEntryAsync(startDate))?.Id ?? 
                    throw new InvalidOperationException($"No TimeEntry found near {startDate} for truncating other activity.");

                if (existingPeriod.StartDate < startDate)
                {
                    existingPeriod.FinishPeriod(startDate, endIdForTruncatedPart);
                }
                else
                {                    
                    existingPeriod.FinishPeriod(startDate, endIdForTruncatedPart);
                }

                if (originalExistingPeriodEndDate > endDate) // Se era null (ativo), também é > endDate
                {
                    var startIdForResumedPeriod = (
                        await GetAssociatedTimeEntryAsync(endDate))?.Id ?? 
                        throw new InvalidOperationException($"No TimeEntry found near {endDate} for resuming other activity.");
                    var resumedPeriod = otherActivity.StartNewPeriod(endDate, startIdForResumedPeriod);

                    if (originalExistingPeriodEndDate != DateTimeOffset.MaxValue)
                    {
                        var endIdForResumedOriginal = originalExistingPeriodEndAssociatedId ?? (await GetAssociatedTimeEntryAsync(originalExistingPeriodEndDate))?.Id ?? throw new InvalidOperationException($"No TimeEntry found near original end date for resuming other activity.");
                        resumedPeriod.FinishPeriod(originalExistingPeriodEndDate, endIdForResumedOriginal);
                    }
                }
            }
            await activityRepository.UpdateAsync(otherActivity, otherActivity.Id);
            await UpsertPeriodsAsync(otherActivity);
        }

        var newRetroPeriod = activity.AddHistoricalPeriod(startDate, startIdForNewRetroPeriod, endDate, endIdForNewRetroPeriod);
        return newRetroPeriod;
    }

    private async Task<TimeEntry?> GetAssociatedTimeEntryAsync(DateTimeOffset dateTime)
    {
        var localDate = dateTime.ToLocalTime();
        var localDateOnly = DateOnly.FromDateTime(localDate.DateTime);

        var timeEntries = await timeEntryRepository.GetByDateAsync(localDateOnly);
        var associatedTimeEntry = timeEntries.OrderByDescending(te => te.EntryDate).FirstOrDefault(te => te.EntryDate <= dateTime);
        return associatedTimeEntry;
    }

    private async Task FinishCurrentPeriod(DateTimeOffset date, Guid endAssociatedTimeEntryId)
    {
        var activeActivitiesToClose = await activityRepository.GetCurrentlyActiveActivitiesAsync(date);
        foreach (var activityToClose in activeActivitiesToClose)
        {
            activityToClose.FinishCurrentPeriod(date, endAssociatedTimeEntryId);
            await UpsertPeriodsAsync(activityToClose);
        }
    }

    private async Task UpsertPeriodsAsync(Activity activity)
    {
        foreach (var item in activity.ActivityPeriods)
        {
            await UpsertPeriodAsync(item);
        }

        var activityWithPeriodDb = await activityRepository.GetByIdWithPeriodsAsync(activity.Id);

        if (activityWithPeriodDb is null) return;

        var periodToDelete = activityWithPeriodDb.ActivityPeriods.Where(ap => activity.ActivityPeriods.All(p => p.Id != ap.Id) && !ap.IsDeleted).ToList();

        foreach (var period in periodToDelete)
        {
            await activityPeriodRepository.DeleteAsync(period.Id);
        }
    }

    private async Task UpsertPeriodAsync(ActivityPeriod activityPeriod)
    {
        if (activityPeriod == null) return;
        var existentPeriod = await activityPeriodRepository.GetByIdAsync(activityPeriod.Id);
        if (existentPeriod != null)
            await activityPeriodRepository.UpdateAsync(activityPeriod, activityPeriod.Id);
        else
            await activityPeriodRepository.AddAsync(activityPeriod);
    }
}
