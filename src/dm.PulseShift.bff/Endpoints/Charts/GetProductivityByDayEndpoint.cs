using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Charts;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.Charts;

public class GetProductivityByDayEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/productivity-by-day", HandleAsync)
           .WithName("GetProductivityByDayChart")
           .WithTags("Charts")
           .Produces<Response<IEnumerable<ProductivityByDayViewModel>>>(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status500InternalServerError)
           .WithDescription("Gets the total productive hours grouped by day of the week.");

    private static async Task<IResult> HandleAsync(IChartReportAppService chartReportAppService)
    {
        var response = await chartReportAppService.GetProductivityByDayOfWeekChartDataAsync();
        return ResponseResult<IEnumerable<ProductivityByDayViewModel>>.CreateResponse(response);
    }
}