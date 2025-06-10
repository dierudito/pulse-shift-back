using AutoMapper;
using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Domain.Dtos;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Domain.Interfaces.Services;
using dm.PulseShift.Infra.CrossCutting.Shared.Helpers;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace dm.PulseShift.Application.AppServices;

public class ActivityAppService(
        IActivityService activityService,
        IActivityRepository activityRepository,
        IActivityWorkCalculatorService workCalculatorService,
        IMapper mapper) : IActivityAppService
{
    public async Task<Response<ActivityResponseViewModel>> CreateActivityAsync(CreateActivityRequestViewModel request)
    {
        try
        {
            var startDate = request.StartDate ?? DateTimeOffset.UtcNow;

            if (startDate.Offset != TimeSpan.Zero) startDate = startDate.ToUniversalTime();
            var activityDto = new ActivityDto(null, request.CardCode, request.Description, request.CardLink, startDate);

            var (createdActivity, initialPeriod) = await activityService.CreateActivityAsync(activityDto);
            await activityRepository.SaveChangesAsync();

            var fullActivity = await activityRepository.GetByIdWithPeriodsAsync(createdActivity.Id);
            var fullActivityJson = JsonSerializer.Serialize(fullActivity, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            var responseData = MapActivityToResponse(fullActivity);
            return new()
            {
                Code = HttpStatusCode.Created,
                Data = responseData,
                Message = "Activity created successfully."
            };
        }
        catch (ArgumentException ex)
        {
            return new()
            {
                Code = HttpStatusCode.BadRequest,
                Message = ex.Message
            };
        }
        catch (InvalidOperationException ex)
        {
            return new Response<ActivityResponseViewModel> { Code = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception)
        {
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred while creating the activity."
            };
        }
    }

    public async Task<Response<ActivityResponseViewModel>> FinishActivityAsync(string cardCode, FinishActivityRequestViewModel request)
    {
        try
        {
            var activity = await activityRepository.GetByCardCodeAsync(cardCode);

            return (activity is null)
                ? new()
                {
                    Code = HttpStatusCode.NotFound,
                    Message = $"Activity with card code '{cardCode}' not found."
                }
                : await FinishActivityAsync(activity.Id, request);
        }
        catch (KeyNotFoundException ex)
        {
            return new() { Code = HttpStatusCode.NotFound, Message = ex.Message };
        }
        catch (InvalidOperationException ex)
        {
            return new() { Code = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception)
        {
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred."
            };
        }
    }

    public async Task<Response<ActivityResponseViewModel>> FinishActivityAsync(Guid activityId, FinishActivityRequestViewModel request)
    {
        try
        {
            var endDate = request.EndDate ?? DateTimeOffset.UtcNow;
            if (endDate.Offset != TimeSpan.Zero) endDate = endDate.ToUniversalTime();
            var finishedPeriod = await activityService.FinishCurrentActivityPeriodAsync(activityId, endDate);
            await activityRepository.SaveChangesAsync();

            var activity = await activityRepository.GetByIdWithPeriodsAsync(activityId);

            var responseData = MapActivityToResponse(activity!);

            return new()
            {
                Code = HttpStatusCode.OK,
                Data = responseData,
                Message = "Current activity period finished successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new()
            {
                Code = HttpStatusCode.NotFound,
                Message = ex.Message
            };
        }
        catch (InvalidOperationException ex) // e.g., Activity already finished
        {
            return new()
            {
                Code = HttpStatusCode.BadRequest,
                Message = ex.Message
            };
        }
        catch (ArgumentException ex)
        {
            return new()
            {
                Code = HttpStatusCode.BadRequest,
                Message = ex.Message
            };
        }
        catch (Exception)
        {
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred while finishing the activity."
            };
        }
    }

    public async Task<Response<ActivityResponseViewModel>> StartActivityAsync(string cardCode, StartActivityRequestViewModel request)
    {
        try
        {
            var activity = await activityRepository.GetByCardCodeAsync(cardCode);

            return (activity == null)
                ? new()
                {
                    Code = HttpStatusCode.NotFound,
                    Message = $"Activity with card code '{cardCode}' not found."
                }
                : await StartActivityAsync(activity.Id, request);
        }
        catch (KeyNotFoundException ex)
        {
            return new() { Code = HttpStatusCode.NotFound, Message = ex.Message };
        }
        catch (InvalidOperationException ex)
        {
            return new() { Code = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception)
        {
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred."
            };
        }
    }

    public async Task<Response<ActivityResponseViewModel>> StartActivityAsync(Guid activityId, StartActivityRequestViewModel request)
    {
        try
        {
            var startDate = request.StartDate ?? DateTimeOffset.UtcNow;
            if (startDate.Offset != TimeSpan.Zero) startDate = startDate.ToUniversalTime();

            var newPeriod = await activityService.StartActivityAsync(activityId, startDate);
            await activityRepository.SaveChangesAsync();

            var activity = await activityRepository.GetByIdWithPeriodsAsync(activityId);
            var activeActivitiesToCloseJson = JsonSerializer.Serialize(activity, new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.Preserve
            });
            var responseData = MapActivityToResponse(activity!);

            return new()
            {
                Code = HttpStatusCode.OK,
                Data = responseData,
                Message = "New period started for the activity successfully."
            };
        }
        catch (KeyNotFoundException ex)
        {
            return new() { Code = HttpStatusCode.NotFound, Message = ex.Message };
        }
        catch (InvalidOperationException ex)
        {
            return new() { Code = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (Exception e)
        {
            return new()
            {
                Code = HttpStatusCode.InternalServerError,
                Message = "An unexpected error occurred."
            };
        }
    }
    public async Task<Response<ActivityWorkDetailsResponseViewModel>> GetActivityWorkDetailsAsync(Guid activityId)
    {
        try
        {
            var activity = await activityRepository.GetByIdWithPeriodsAsync(activityId);
            if (activity == null || activity.IsDeleted)
            {
                return new Response<ActivityWorkDetailsResponseViewModel> { Code = HttpStatusCode.NotFound, Message = "Activity not found." };
            }

            TimeSpan totalWorkedTimeSpan = await workCalculatorService.CalculateEffectiveWorkTimeAsync(activity);
            decimal totalWorkedHours = Math.Round((decimal)totalWorkedTimeSpan.TotalHours, 2); // Arredondando para 2 casas decimais

            // Mapear os períodos da atividade para o DTO de período
            var periodResponses = mapper.Map<IEnumerable<ActivityPeriodResponseViewModel>>(activity.ActivityPeriods.Where(p => !p.IsDeleted));

            var responseData = new ActivityWorkDetailsResponseViewModel(
                activity.Id,
                activity.Description,
                activity.CardCode,
                activity.CardLink,
                FormatHelper.FormatNumberToBrazilianString(totalWorkedHours),
                FormatHelper.FormatDateTimeOffsetToBrazilianString(activity.FirstOverallStartDate),
                FormatHelper.FormatDateTimeOffsetToBrazilianString(activity.LastOverallEndDate),
                FormatHelper.FormatDateTimeOffsetToBrazilianString(activity.CreatedAt),
                FormatHelper.FormatDateTimeOffsetToBrazilianString(activity.UpdatedAt),
                activity.IsCurrentlyActive,
                periodResponses
            );

            return new Response<ActivityWorkDetailsResponseViewModel> { Code = HttpStatusCode.OK, Data = responseData };
        }
        catch (Exception ex)
        {
            // Log ex
            return new Response<ActivityWorkDetailsResponseViewModel> { Code = HttpStatusCode.InternalServerError, Message = $"An error occurred: {ex.Message}" };
        }
    }

    public async Task<Response<ActivityWorkDetailsResponseViewModel>> GetActivityWorkDetailsByCardCodeAsync(string cardCode)
    {
        try
        {
            var activity = await activityRepository.GetByCardCodeWithPeriodsAsync(cardCode);
            if (activity == null || activity.IsDeleted)
            {
                return new Response<ActivityWorkDetailsResponseViewModel> { Code = HttpStatusCode.NotFound, Message = "Activity not found for the given CardCode." };
            }

            return await GetActivityWorkDetailsAsync(activity.Id);
        }
        catch (Exception ex)
        {
            // Log ex
            return new Response<ActivityWorkDetailsResponseViewModel> { Code = HttpStatusCode.InternalServerError, Message = $"An error occurred: {ex.Message}" };
        }
    }

    private ActivityResponseViewModel MapActivityToResponse(Activity activity) => mapper.Map<ActivityResponseViewModel>(activity);

    public async Task<Response<IEnumerable<ActivitySummaryResponseViewModel>>> GetActivitySummaryAsync() 
    {
        try
        {
            var activities = await activityRepository.GetAllAsync();
            if (activities == null || !activities.Any())
            {
                return new()
                {
                    Code = HttpStatusCode.OK,
                    Message = "No activities found."
                };
            }
            var responseData = mapper.Map<IEnumerable<ActivitySummaryResponseViewModel>>(activities.OrderByDescending(a => a.UpdatedAt));
            return new()
            {
                Code = HttpStatusCode.OK,
                Data = responseData,
                Message = "Activity summary retrieved successfully."
            };
        }
        catch (Exception ex)
        {
            // Log ex
            return new Response<IEnumerable<ActivitySummaryResponseViewModel>>
            {
                Code = HttpStatusCode.InternalServerError,
                Message = $"An error occurred while retrieving the activity summary: {ex.Message}"
            };
        }
    }

    public async Task<Response<ActivityResponseViewModel>> AddRetroactiveActivityPeriodAsync(CreateRetroactiveActivityPeriodRequestViewModel request)
    {

        try
        {
            var startDate = request.StartDate;
            var endDate = request.EndDate;
            if (startDate.Offset != TimeSpan.Zero) startDate = startDate.ToUniversalTime();
            if (endDate.Offset != TimeSpan.Zero) endDate = endDate.ToUniversalTime();

            var addedPeriod = await activityService.AddRetroactivePeriodAsync(request.CardCode, startDate, endDate);

            await activityRepository.SaveChangesAsync();

            var targetActivity = await activityRepository.GetByCardCodeWithPeriodsAsync(request.CardCode);
            if (targetActivity == null)
            {
                return new() { Code = HttpStatusCode.InternalServerError, Message = "Failed to retrieve activity after adding retroactive period." };
            }

            var responseData = MapActivityToResponse(targetActivity);

            return new()
            {
                Code = HttpStatusCode.OK,
                Data = responseData,
                Message = "Retroactive activity period added successfully and other activities adjusted."
            };
        }
        catch (ArgumentException ex)
        {
            return new Response<ActivityResponseViewModel> { Code = HttpStatusCode.BadRequest, Message = ex.Message };
        }
        catch (KeyNotFoundException ex)
        {
            return new Response<ActivityResponseViewModel> { Code = HttpStatusCode.NotFound, Message = ex.Message };
        }
        catch (InvalidOperationException ex) // Para regras de negócio violadas (ex: sobreposição na mesma atividade)
        {
            return new Response<ActivityResponseViewModel> { Code = HttpStatusCode.Conflict, Message = ex.Message };
        }
        catch (Exception ex)
        {
            // TODO: Logar a exceção ex
            return new Response<ActivityResponseViewModel> { Code = HttpStatusCode.InternalServerError, Message = $"An unexpected error occurred: {ex.Message}" };
        }
    }

    public async Task<Response<PaginatedResponseViewModel<ActivityPaginatedItemViewModel>>> 
        GetActivitiesPaginatedAsync(DateTimeOffset filterStartDate, DateTimeOffset filterEndDate, int pageNumber, int pageSize)
    {
        try
        {
            if (filterStartDate >= filterEndDate)
            {
                return new Response<PaginatedResponseViewModel<ActivityPaginatedItemViewModel>>
                {
                    Code = HttpStatusCode.BadRequest,
                    Message = "Filter start date must be before filter end date."
                };
            }
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            filterStartDate = filterStartDate.ToUniversalTime();
            filterEndDate = filterEndDate.ToUniversalTime();

            var (activitiesData, totalRecords) = await activityRepository.GetActivitiesByDateRangePaginatedAsync(
                filterStartDate, filterEndDate, pageNumber, pageSize);

            var activityItems = new List<ActivityPaginatedItemViewModel>();

            foreach (var activity in activitiesData)
            {
                TimeSpan totalWorkedTimeSpan = await workCalculatorService.CalculateEffectiveWorkTimeAsync(activity);
                decimal totalWorkedHoursDecimal = (decimal)totalWorkedTimeSpan.TotalHours;
                var formattedTotalWorkedHours = FormatHelper.FormatNumberToBrazilianString(totalWorkedHoursDecimal);

                string? formattedLastEndDate = activity.LastOverallEndDate.HasValue
                    ? FormatHelper.FormatDateTimeOffsetToBrazilianString(activity.LastOverallEndDate)
                    : null;

                activityItems.Add(new ActivityPaginatedItemViewModel(
                    activity.Id,
                    activity.CardCode,
                    activity.Description,
                    formattedTotalWorkedHours,
                    activity.IsCurrentlyActive,
                    formattedLastEndDate
                ));
            }

            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            var paginatedData = new PaginatedResponseViewModel<ActivityPaginatedItemViewModel>(
                pageNumber,
                pageSize,
                totalPages,
                totalRecords,
                activityItems
            );

            return new Response<PaginatedResponseViewModel<ActivityPaginatedItemViewModel>>
            {
                Code = HttpStatusCode.OK,
                Data = paginatedData
            };
        }
        catch (Exception ex)
        {
            return new Response<PaginatedResponseViewModel<ActivityPaginatedItemViewModel>>
            {
                Code = HttpStatusCode.InternalServerError,
                Message = $"An error occurred while fetching paginated activities: {ex.Message}"
            };
        }
    }
}
