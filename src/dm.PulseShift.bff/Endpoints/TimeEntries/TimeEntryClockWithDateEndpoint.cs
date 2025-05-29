using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.TimeEntries;

public class TimeEntryClockWithDateEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/clock/{date}", HandleAsync)
            .WithName("TimeEntryClockWithDate")
            .WithTags("Time Entry")
            .Produces<Response<TimeEntryResponseViewModel>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Create a new time entry with a specific date");
    private static async Task<IResult> HandleAsync(ITimeEntryAppService appService, DateTime date)
    {
        var response = await appService.CreateAsync(date);
        return ResponseResult<TimeEntryResponseViewModel>.CreateResponse(response);
    }
}