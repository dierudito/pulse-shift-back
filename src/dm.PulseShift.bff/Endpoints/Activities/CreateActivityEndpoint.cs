using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class CreateActivityEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/", HandleAsync)
            .WithName("CreateActivity")
            .WithTags("Activity")
            .Produces<Response<ActivityResponseViewModel>>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Creates a new activity and starts its initial period.");

    private static async Task<IResult> HandleAsync(
        IActivityAppService appService,
        [FromBody] CreateActivityRequestViewModel request)
    {
        var response = await appService.CreateActivityAsync(request);
        return ResponseResult<ActivityResponseViewModel>.CreateResponse(response);
    }
}