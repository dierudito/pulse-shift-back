using System.Net;

namespace dm.PulseShift.Infra.CrossCutting.Shared;

public class ApiConfigurations
{
    public const HttpStatusCode DefaultStatusCode = HttpStatusCode.OK;
    public static string ConnectionString { get; set; } = string.Empty;
    public static string CorsPolicyName { get; set; } = "PulseShiftPolicy";
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 25;
    public static string BackendUrl { get; set; } = "https://localhost:5001";
    public static string FrontendUrl { get; set; } = "http://localhost:5174";
    public const string RouterTimeEntry = "/time-entries";
    public const string RouterDayOff = "/days-off";
    public const string RouterActivity = "/activities";
    public const string RouterWorkSummary = "/work-summary";
    public const string RouterReports = "/reports";
}
