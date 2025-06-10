using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.bff.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace dm.PulseShift.bff.Endpoints.Activities;

public class GetActivitiesPaginatedEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) =>
        app.MapGet("/paginated", HandleAsync)
            .WithName("GetActivitiesPaginated")
            .WithTags("Activity")
            .Produces<Response<PaginatedResponseViewModel<ActivityPaginatedItemViewModel>>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithDescription("Gets a paginated list of activities within a specified date range, ordered by active status and then by last overall end date descending.");

    private static async Task<IResult> HandleAsync(
        IActivityAppService appService,
        [FromQuery] DateTimeOffset filterStartDate,
        [FromQuery] DateTimeOffset filterEndDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var response = await appService.GetActivitiesPaginatedAsync(filterStartDate, filterEndDate, pageNumber, pageSize);
        return ResponseResult<PaginatedResponseViewModel<ActivityPaginatedItemViewModel>>.CreateResponse(response);
    }
}
