using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.WorkSummary;

public class GetTotalWorkedHoursInRangeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/hours", HandleAsync)
            .WithName("GetTotalWorkedHoursInRange")
            .WithTags("Work Summary") // Novo Tag
            .Produces<Response<TotalWorkHoursResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Calculates the total effective work hours based on time entries and activities within a specified date range.");

    private static async Task<IResult> HandleAsync(
        IWorkSummaryAppService appService, // Ou IWorkSummaryAppService se você criou um separado
        [FromQuery] DateTimeOffset startDate,
        [FromQuery] DateTimeOffset endDate)
    {
        var request = new WorkHoursQueryRequestViewModel(startDate, endDate);
        var response = await appService.GetTotalWorkedHoursInRangeAsync(request);
        return ResponseResult<TotalWorkHoursResponseViewModel>.CreateResponse(response);
    }
}