using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.DaysOff;

public class CreateDayOffEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/", HandleAsync)
            .WithName("CreateDayOff")
            .WithTags("Day Off")
            .Produces<Response<bool>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Create a new day off");

    private static async Task<IResult> HandleAsync(IDayOffAppService appService, [FromBody] CreateDayOffRequestViewModel requestViewModel)
    {
        var response = await appService.AddDayOffAsync(requestViewModel);
        return ResponseResult<bool>.CreateResponse(response);
    }
}