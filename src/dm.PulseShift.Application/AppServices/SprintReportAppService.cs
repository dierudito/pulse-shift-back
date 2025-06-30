using dm.PulseShift.Application.Interfaces;
using dm.PulseShift.Application.ViewModels.Requests;
using dm.PulseShift.Application.ViewModels.Responses.Base;
using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Enums;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.CrossCutting.Shared.Helpers;
using System.Net;

namespace dm.PulseShift.Application.AppServices;

public class SprintReportAppService(ISprintReportRepository sprintReportRepository) : ISprintReportAppService
{
    public async Task<Response<Guid>> ImportSprintReportAsync(SprintReportRequestViewModel request)
    {
        var startDate = request.StartDate.ConvertPortugueseMonthDayToDateTime();
        var endDate = request.EndDate.ConvertPortugueseMonthDayToDateTime();

        var sprintReport = new SprintReport
        {
            NomeSprint = request.SprintName,
            NumeroSprint = request.SprintNumber,
            DataInicio = startDate,
            DataFim = endDate
        };

        sprintReport.WorkItems = MapWorkItems(request.Activities, sprintReport);

        await sprintReportRepository.AddAsync(sprintReport);
        await sprintReportRepository.SaveChangesAsync();

        return new Response<Guid>
        {
            Code = HttpStatusCode.Created,
            Data = sprintReport.Id,
            Message = "Sprint report imported successfully."
        };
    }

    private static List<WorkItem> MapWorkItems(IEnumerable<WorkItemRequestViewModel> requestItems, SprintReport report, WorkItem? parent = null)
    {
        var workItems = new List<WorkItem>();
        foreach (var requestItem in requestItems)
        {
            var workItem = new WorkItem
            {
                Nome = requestItem.Name,
                Descricao = requestItem.Description,
                Responsavel = requestItem.Responsible,
                Status = ParseEnum<WorkItemStatus>(requestItem.Status.Replace(" ", "")),
                Tipo = ParseEnum<WorkItemType>(requestItem.Type.Replace(" ", "")),
                SprintReport = report,
                ParentWorkItem = parent
            };

            if (requestItem.SubActivities.Count != 0)
            {
                workItem.SubWorkItems = MapWorkItems(requestItem.SubActivities, report, workItem);
            }
            workItems.Add(workItem);
        }
        return workItems;
    }

    private static T ParseEnum<T>(string value) where T : struct
    {
        if (Enum.TryParse<T>(value, true, out var result))
        {
            return result;
        }
        return default;
    }
}