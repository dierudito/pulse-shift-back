using dm.PulseShift.Domain.Dtos;
using dm.PulseShift.Domain.Entities;

namespace dm.PulseShift.Domain.Interfaces.Services;

public interface IActivityService
{
    Task<(Activity Activity, ActivityPeriod InitialPeriod)> CreateActivityAsync(ActivityDto activityDto); 
    Task<ActivityPeriod> StartActivityAsync(Guid activityId, DateTime startDate);
    Task<ActivityPeriod> FinishCurrentActivityPeriodAsync(Guid activityId, DateTime endDate);
    Task<ActivityPeriod> AddRetroactivePeriodAsync(string cardCode, DateTime startDate, DateTime endDate);
}
