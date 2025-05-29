using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;

namespace dm.PulseShift.bff.Endpoints.TimeEntries;

public class TimeEntryPeriodDurationEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/duration/period", HandleAsync)
            .WithName("TimeEntryPeriodDuration")
            .WithTags("Time Entry Period Duration")
            .Produces<Response<GetPeriodDurationResponseViewModel>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Get time entry duration for a period");
    private static async Task<IResult> HandleAsync(ITimeEntryAppService appService, DateOnly startDate, DateOnly endDate)
    {
        var response = await appService.GetPeriodDurationAsync(startDate, endDate);
        return ResponseResult<GetPeriodDurationResponseViewModel>.CreateResponse(response);
    }
}