using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;

namespace dm.PulseShift.Application.Interfaces;

public interface IWorkSummaryAppService
{
    Task<Response<TotalWorkHoursResponseViewModel>> GetTotalWorkedHoursInRangeAsync(WorkHoursQueryRequestViewModel request);
}