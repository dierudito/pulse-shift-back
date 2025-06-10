using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class FinishActivityEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPatch("/{cardCode}/finish", HandleAsync)
            .WithName("FinishActivity")
            .WithTags("Activity")
            .Produces<Response<ActivityResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Marks the current open work period of an activity as finished.");

    private static async Task<IResult> HandleAsync(
        [FromRoute] string cardCode,
        IActivityAppService appService,
        [FromBody] FinishActivityRequestViewModel request)
    {
        var response = await appService.FinishActivityAsync(cardCode, request);
        return ResponseResult<ActivityResponseViewModel>.CreateResponse(response);
    }
}