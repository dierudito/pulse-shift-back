using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class GetActivityWorkDetailsByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/id/{activityId:guid}/work-details", HandleAsync)
            .WithName("GetActivityWorkDetailsById")
            .WithTags("Activity")
            .Produces<Response<ActivityWorkDetailsResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Gets detailed work time information for an activity by its GUID ID.");

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid activityId,
        IActivityAppService appService)
    {
        var response = await appService.GetActivityWorkDetailsAsync(activityId);
        return ResponseResult<ActivityWorkDetailsResponseViewModel>.CreateResponse(response);
    }
}