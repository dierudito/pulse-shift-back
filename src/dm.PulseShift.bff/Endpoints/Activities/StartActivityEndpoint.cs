using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class StartActivityEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/{cardCode}/start", HandleAsync)
            .WithName("StartActivity_CardCode")
            .WithTags("Activity")
            .Produces<Response<ActivityResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Starts a new work period for an existing activity that was previously finished.");

        app.MapPost("/{activityId:guid}/start", HandleGuidAsync)
            .WithName("StartActivity")
            .WithTags("Activity")
            .Produces<Response<ActivityResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Starts a new work period for an existing activity that was previously finished.");
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] string cardCode,
        IActivityAppService appService,
        [FromBody] StartActivityRequestViewModel request)
    {
        var response = await appService.StartActivityAsync(cardCode, request);
        return ResponseResult<ActivityResponseViewModel>.CreateResponse(response);
    }

    private static async Task<IResult> HandleGuidAsync(
        [FromRoute] Guid activityId,
        IActivityAppService appService,
        [FromBody] StartActivityRequestViewModel request)
    {
        var response = await appService.StartActivityAsync(activityId, request);
        return ResponseResult<ActivityResponseViewModel>.CreateResponse(response);
    }
}
