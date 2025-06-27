using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;

namespace dm.PulseShift.Application.Interfaces;
public interface ISprintReportAppService
{
    Task<Response<Guid>> ImportSprintReportAsync(SprintReportRequestViewModel request);
}