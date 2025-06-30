using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.TimeEntries;

public class TimeEntryScheduleByDateEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/schedule/date", HandleAsync)
            .WithName("TimeEntryScheduleByDate")
            .WithTags("Time Entry")
            .Produces<Response<GetWorkScheduleResponseViewModel>>() // Crie um ViewModel adequado para a resposta
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Get the work schedule (expected/calculated time entries) by date");

    private static async Task<IResult> HandleAsync(ITimeEntryAppService appService, [FromQuery] DateOnly date)
    {
        var response = await appService.GetWorkScheduleByDateAsync(date);
        return ResponseResult<GetWorkScheduleResponseViewModel>.CreateResponse(response);
    }
}