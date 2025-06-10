using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;

namespace dm.PulseShift.Application.Interfaces;
public interface ITimeEntryAppService
{
    Task<Response<TimeEntryResponseViewModel>> CreateAsync();
    Task<Response<TimeEntryResponseViewModel>> CreateAsync(DateTime date);
    Task<Response<GetTodaysDurationResponseViewModel>> GetTodaysDurationAsync();
    Task<Response<GetDurationResponseViewModel>> GetDurationAsync(DateOnly date);
    Task<Response<GetPeriodDurationResponseViewModel>> GetPeriodDurationAsync(DateOnly startDate, DateOnly endDate);
    Task<Response<GetWorkScheduleResponseViewModel>> GetWorkScheduleByDateAsync(DateOnly date);
    Task<Response<IEnumerable<GetTimeEntriesPerDay>>> GetTimeEntriesPerDayAsync(DateOnly date);
}