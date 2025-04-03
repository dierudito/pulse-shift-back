using System.Net;

namespace dm.PulseShift.Infra.CrossCutting.Shared;

public class ApiConfigurations
{
    public const HttpStatusCode DefaultStatusCode = HttpStatusCode.OK;
    public static string ConnectionString { get; set; } = string.Empty;
    public static string CorsPolicyName { get; set; } = string.Empty;
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 25;
    public static string BackendUrl { get; set; } = string.Empty;
    public static string FrontendUrl { get; set; } = string.Empty;
    public const string RouterTimeEntry = "time-entry";
}
