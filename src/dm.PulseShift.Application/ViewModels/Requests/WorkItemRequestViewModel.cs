using System.Text.Json.Serialization;

namespace dm.PulseShift.Application.ViewModels.Requests;

public record WorkItemRequestViewModel
{
    [JsonPropertyName("descricao")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("responsavel")]
    public string Responsible { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("subAtividades")]
    public List<WorkItemRequestViewModel> SubActivities { get; init; } = [];

    [JsonPropertyName("tipo")]
    public string Type { get; init; } = string.Empty;
}
