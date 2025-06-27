using System.Text.Json.Serialization;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record SprintReportRequestViewModel
{
    [JsonPropertyName("atividades")]
    public List<WorkItemRequestViewModel> Activities { get; init; } = new();

    [JsonPropertyName("dataFim")]
    public string EndDate { get; init; } = string.Empty;

    [JsonPropertyName("dataInicio")]
    public string StartDate { get; init; } = string.Empty;

    [JsonPropertyName("nomeSprint")]
    public string SprintName { get; init; } = string.Empty;

    [JsonPropertyName("numeroSprint")]
    public int SprintNumber { get; init; }
}