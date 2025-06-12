using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Domain.Interfaces.Services;
using dm.PulseShift.Infra.CrossCutting.Shared.Helpers;
using System.Net;

namespace dm.PulseShift.Application.AppServices;

public class WorkSummaryAppService(IActivityWorkCalculatorService activityWorkCalculatorService) : IWorkSummaryAppService
{
    public async Task<Response<TotalWorkHoursResponseViewModel>> GetTotalWorkedHoursInRangeAsync(WorkHoursQueryRequestViewModel request)
    {
		try
        {
            if (request.StartDate >= request.EndDate) 
                return new() { Code = HttpStatusCode.BadRequest, Message = "Start date must be before end date." };

            var calculationResult = await activityWorkCalculatorService.CalculateTotalEffectiveActivityTimeInRangeAsync(request.StartDate, request.EndDate);

            var responseData = new TotalWorkHoursResponseViewModel(
                FormatHelper.FormatNumberToBrazilianString((decimal)calculationResult.TotalWorkHoursFromEntries.TotalHours),
                FormatHelper.FormatNumberToBrazilianString((decimal)calculationResult.TotalWorkCoveredByActivities.TotalHours),
                FormatHelper.FormatDateTimeToBrazilianString(request.StartDate),
                FormatHelper.FormatDateTimeToBrazilianString(request.EndDate)
            );

            return new() { Code = HttpStatusCode.OK, Data = responseData };
        }
		catch (Exception ex)
		{
            return new () { Code = HttpStatusCode.InternalServerError, Message = $"An error occurred: {ex.Message}" };
        }
    }
}
