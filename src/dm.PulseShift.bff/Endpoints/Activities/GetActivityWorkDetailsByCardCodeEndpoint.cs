using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class GetActivityWorkDetailsByCardCodeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/{cardCode}/work-details", HandleAsync)
            .WithName("GetActivityWorkDetailsByCardCode")
            .WithTags("Activity")
            .Produces<Response<ActivityWorkDetailsResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Gets detailed work time information for an activity by its CardCode.");

    private static async Task<IResult> HandleAsync(
        [FromRoute] string cardCode,
        IActivityAppService appService)
    {
        var response = await appService.GetActivityWorkDetailsByCardCodeAsync(cardCode);
        return ResponseResult<ActivityWorkDetailsResponseViewModel>.CreateResponse(response);
    }
}