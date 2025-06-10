using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Reports;

namespace dm.PulseShift.Application.Interfaces;
public interface IReportAppService
{
    Task<Response<PeriodReportResponseViewModel>> GetPeriodReportAsync(PeriodReportRequestViewModel request);
}