using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;

namespace dm.PulseShift.Application.Interfaces;

public interface IActivityAppService
{
    Task<Response<ActivityResponseViewModel>> CreateActivityAsync(CreateActivityRequestViewModel request);
    Task<Response<ActivityResponseViewModel>> StartActivityAsync(string cardCode, StartActivityRequestViewModel request);
    Task<Response<ActivityResponseViewModel>> StartActivityAsync(Guid activityId, StartActivityRequestViewModel request);
    Task<Response<ActivityResponseViewModel>> FinishActivityAsync(Guid activityId, FinishActivityRequestViewModel request);
    Task<Response<ActivityResponseViewModel>> FinishActivityAsync(string cardCode, FinishActivityRequestViewModel request);
    Task<Response<ActivityWorkDetailsResponseViewModel>> GetActivityWorkDetailsAsync(Guid activityId);
    Task<Response<ActivityWorkDetailsResponseViewModel>> GetActivityWorkDetailsByCardCodeAsync(string cardCode);
    Task<Response<IEnumerable<ActivitySummaryResponseViewModel>>> GetActivitySummaryAsync();
    Task<Response<ActivityResponseViewModel>> AddRetroactiveActivityPeriodAsync(CreateRetroactiveActivityPeriodRequestViewModel request);     
    Task<Response<PaginatedResponseViewModel<ActivityPaginatedItemViewModel>>> GetActivitiesPaginatedAsync(
        DateTimeOffset filterStartDate,
        DateTimeOffset filterEndDate,
        int pageNumber,
        int pageSize);
}