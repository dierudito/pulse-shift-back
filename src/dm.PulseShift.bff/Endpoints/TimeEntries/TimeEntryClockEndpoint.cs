using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.TimeEntries;

public class TimeEntryClockEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/clock", HandleAsync)
            .WithName("TimeEntryClock")
            .WithTags("Time Entry")
            .Produces<Response<TimeEntryResponseViewModel>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Create a new time entry");

    private static async Task<IResult> HandleAsync(ITimeEntryAppService appService)
    {
        var response = await appService.CreateAsync();
        return ResponseResult<TimeEntryResponseViewModel>.CreateResponse(response);
    }
}