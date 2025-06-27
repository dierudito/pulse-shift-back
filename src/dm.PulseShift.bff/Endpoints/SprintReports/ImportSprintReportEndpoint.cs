using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.SprintReports;

public class ImportSprintReportEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapPost("/import", HandleAsync)
           .WithName("ImportSprintReport")
           .WithTags("Sprint Reports")
           .Produces<Response<Guid>>(StatusCodes.Status201Created)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError)
           .WithDescription("Receives and persists a full sprint report with its activities and sub-activities.");

    private static async Task<IResult> HandleAsync(
        ISprintReportAppService appService,
        [FromBody] SprintReportRequestViewModel request)
    {
        var response = await appService.ImportSprintReportAsync(request);
        return ResponseResult<Guid>.CreateResponse(response);
    }
}