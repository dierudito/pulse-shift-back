using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class GetActivitiesSummaryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/summary", HandleAsync)
            .WithName("GetActivitiesSummary")
            .WithTags("Activity")
            .Produces<Response<IEnumerable<ActivitySummaryResponseViewModel>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Gets a summary of all activities.");

    private static async Task<IResult> HandleAsync(
        IActivityAppService appService)
    {
        var response = await appService.GetActivitySummaryAsync();
        return ResponseResult<IEnumerable<ActivitySummaryResponseViewModel>>.CreateResponse(response);
    }
}
