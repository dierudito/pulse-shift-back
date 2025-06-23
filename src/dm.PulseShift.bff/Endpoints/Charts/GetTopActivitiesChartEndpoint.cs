using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Charts;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.Charts;

public class GetTopActivitiesChartEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/top-activities", HandleAsync)
           .WithName("GetTopActivitiesChart")
           .WithTags("Charts")
           .Produces<Response<IEnumerable<TopActivityChartDataViewModel>>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status500InternalServerError)
           .WithDescription("Gets the top 15 most time-consuming activities for chart reporting.");

    private static async Task<IResult> HandleAsync(IChartReportAppService chartReportAppService)
    {
        var response = await chartReportAppService.GetTopTimeConsumingActivitiesChartDataAsync();
        return ResponseResult<IEnumerable<TopActivityChartDataViewModel>>.CreateResponse(response);
    }
}