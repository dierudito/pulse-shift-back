using AutoMapper;
using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Charts;
using dm.PulseShift.Domain.Interfaces.Repositories;
using System.Net;

namespace dm.PulseShift.Application.AppServices;

public class ChartReportAppService(IInsightsRepository insightsRepository, IMapper mapper) : IChartReportAppService
{
    public async Task<Response<IEnumerable<TopActivityChartDataViewModel>>> GetTopTimeConsumingActivitiesChartDataAsync()
    {
        var activitySummaries = await insightsRepository.GetTopTimeConsumingActivitiesAsync();

        var responseData = mapper.Map<IEnumerable<TopActivityChartDataViewModel>>(activitySummaries);

        return new Response<IEnumerable<TopActivityChartDataViewModel>>
        {
            Code = HttpStatusCode.OK,
            Data = responseData
        };
    }
    public async Task<Response<IEnumerable<ProductivityByDayViewModel>>> GetProductivityByDayOfWeekChartDataAsync()
    {
        var dailyProductivitySummaries = await insightsRepository.GetProductivityByDayOfWeekAsync();
        var responseData = mapper.Map<IEnumerable<ProductivityByDayViewModel>>(dailyProductivitySummaries);
        return new Response<IEnumerable<ProductivityByDayViewModel>>
        {
            Code = HttpStatusCode.OK,
            Data = responseData
        };
    }
}