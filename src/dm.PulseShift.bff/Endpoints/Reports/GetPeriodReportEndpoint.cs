using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Application.ViewModels.Responses.Reports;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Reports;

public class GetPeriodReportEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/", HandleAsync)
            .WithName("GetPeriodReport")
            .WithTags("Reports")
            .Produces<Response<PeriodReportResponseViewModel>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Generates a work and activity report for a specified period.");

    private static async Task<IResult> HandleAsync(
        IReportAppService reportAppService,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var request = new PeriodReportRequestViewModel(startDate, endDate.ToLocalTime());
        var response = await reportAppService.GetPeriodReportAsync(request);
        return ResponseResult<PeriodReportResponseViewModel>.CreateResponse(response);
    }
}