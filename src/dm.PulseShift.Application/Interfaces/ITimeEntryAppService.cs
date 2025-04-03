using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;

namespace dm.PulseShift.Application.Interfaces;
public interface ITimeEntryAppService
{
    Task<Response<TimeEntryResponseViewModel>> CreateAsync();
    Task<Response<GetTodaysDurationResponseViewModel>> GetTodaysDurationAsync();
}