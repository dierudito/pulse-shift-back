using dm.PulseShift.Infra.CrossCutting.Shared;
using System.Net;
using System.Text.Json.Serialization;

namespace dm.PulseShift.Application.ViewModels.Responses.Base;

public class Response<TData>
{
    [JsonIgnore]
    public HttpStatusCode Code { get; set; } = ApiConfigurations.DefaultStatusCode;

    [JsonConstructor]
    public Response() => Code = ApiConfigurations.DefaultStatusCode;

    public Response(TData? data, HttpStatusCode code = ApiConfigurations.DefaultStatusCode, string? message = null)
    {
        Data = data;
        Code = code;
        Message = message;
    }

    public TData? Data { get; set; }

    public string? Message { get; set; }

    [JsonIgnore]
    public bool IsSuccess => (int)Code is >= 200 and <= 299;
}