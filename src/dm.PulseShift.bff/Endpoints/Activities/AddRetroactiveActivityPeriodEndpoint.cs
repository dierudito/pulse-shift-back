using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class AddRetroactiveActivityPeriodEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/retroactive-period", HandleAsync)
            .WithName("AddRetroactiveActivityPeriod")
            .WithTags("Activity")
            .Produces<Response<ActivityResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Adds a historical (retroactive) period to an existing activity and adjusts other overlapping activities.");

    private static async Task<IResult> HandleAsync(
        IActivityAppService appService,
        [FromBody] CreateRetroactiveActivityPeriodRequestViewModel request)
    {
        var response = await appService.AddRetroactiveActivityPeriodAsync(request);
        return ResponseResult<ActivityResponseViewModel>.CreateResponse(response);
    }
}