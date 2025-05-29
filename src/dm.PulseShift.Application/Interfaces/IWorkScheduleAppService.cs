using dm.PulseShift.Application.ViewModels.Requests;

namespace dm.PulseShift.Application.Interfaces;

public interface IWorkScheduleAppService
{
    Task<bool> AddWorkScheduleAsync(CreateWorkScheduleRequestViewModel requestViewModel);
    Task<long> GetTimeForNextExecAsync();
}
