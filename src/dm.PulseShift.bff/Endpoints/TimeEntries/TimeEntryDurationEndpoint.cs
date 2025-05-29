using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.TimeEntries;

public class TimeEntryDurationEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/duration/date", HandleAsync)
            .WithName("TimeEntryDuration")
            .WithTags("Time Entry Duration")
            .Produces<Response<GetDurationResponseViewModel>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Get time entry duration by date");

    private static async Task<IResult> HandleAsync(ITimeEntryAppService appService, [FromQuery] DateOnly date)
    {
        var response = await appService.GetDurationAsync(date);
        return ResponseResult<GetDurationResponseViewModel>.CreateResponse(response);
    }
}