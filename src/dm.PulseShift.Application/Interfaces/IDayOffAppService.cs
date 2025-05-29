using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;

namespace dm.PulseShift.Application.Interfaces;

public interface IDayOffAppService
{
    Task<Response<bool>> AddDayOffAsync(CreateDayOffRequestViewModel requestViewModel);
    Task<bool> IsDayOffAsync(DateOnly date);
    Task<long> CalcTimeUntilNextExecAsync(DateOnly date);
}