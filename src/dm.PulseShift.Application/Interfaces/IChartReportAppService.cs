using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Charts;

namespace dm.PulseShift.Application.Interfaces;

public interface IChartReportAppService
{
    Task<Response<IEnumerable<TopActivityChartDataViewModel>>> GetTopTimeConsumingActivitiesChartDataAsync();
    Task<Response<IEnumerable<ProductivityByDayViewModel>>> GetProductivityByDayOfWeekChartDataAsync();
}